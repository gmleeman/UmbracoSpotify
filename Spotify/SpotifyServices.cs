using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Services;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using UmbracoSpotify.Web.Models;
using UmbracoSpotify.Spotify;
using UmbracoSpotify.Recommendations;

namespace UmbracoSpotify
{
    public class SpotifyServices 
    {
        private string _clientid = "d81190a7ebee43c09effa90bafeffe73";
        private string _redir = "http://localhost:10542/callback";

        private string _sContentType = "application/json";
        private JsonSerializerSettings _serialSettings;

        private CallbackModel _callback;

        private readonly AppSession _mySession;
        private readonly HttpClient _httpClient;
        public SpotifyServices(HttpSpotify httpSpot, SessionStateProvider state)
        {
            _httpClient = httpSpot.SpotifyClient;
            _serialSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
                ,MissingMemberHandling = MissingMemberHandling.Ignore
                ,ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            _mySession = new AppSession(state);
        }

        private List<string> _listGenres = new List<string>();
        public List<string> genres
        {
            get
            {
                
                var task = LoadSeedGenresAsync();
                return _listGenres;
            }
        }

        /// <summary>
        /// Cache forever, as they never change. (Well, we can assume that)
        /// from https://developer.spotify.com/web-api/get-recommendations/
        /// </summary>
        /// <returns></returns>
        public async Task<int> LoadSeedGenresAsync()
        {
            if (_listGenres == null || _listGenres.Count == 0)
                if (_mySession.CSession["SpotifyGenreList"] != null)
                {
                    var cacheGenres = (List<string>)_mySession.CSession["SpotifyGenreList"];
                    _listGenres = cacheGenres;
                }
            
            if (_listGenres == null || _listGenres.Count == 0)
            {
                string endpoint = "recommendations/available-genre-seeds";
                var objGenres = await GetFromSpotifyAsync<SpotifyRecommendationSeedGenres>(endpoint);
                _listGenres.AddRange(objGenres.genres);
                _mySession.CSession["SpotifyGenreList"] = _listGenres;
            }
            return _listGenres.Count;
        }

        private void SetHTTPClientHeaders()
        {
            if (_mySession.CSession["SpotifyAccess"]!=null)
            {
                _callback = (CallbackModel)_mySession.CSession["SpotifyAccess"];
            }
            
            lock (_httpClient)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                if (MediaTypeWithQualityHeaderValue.TryParse(_sContentType, out MediaTypeWithQualityHeaderValue mtqhv))
                {
                    _httpClient.DefaultRequestHeaders.Accept.Add(mtqhv);
                }
                if (_callback!=null)
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _callback.accessToken);
            }
        }

        private string GetSearchUri(string searchquery, string type, int limit, int offset)
        {
            string converted = HttpUtility.HtmlEncode(searchquery);
            var endpoint = new StringBuilder();
            endpoint.Append($"search?q={converted}");
            endpoint.Append($"&type={type}");
            endpoint.Append($"&market=from_token");
            endpoint.Append($"&limit={limit}");
            endpoint.Append($"&offset={offset}");
            return endpoint.ToString();
        }

        public string GetAuthorizeUri()
        {
            StringBuilder builder = new StringBuilder("https://accounts.spotify.com/authorize/?");
            builder.Append("client_id=" + _clientid);
            builder.Append("&response_type=token");
            builder.Append("&redirect_uri=" + _redir);
            builder.Append("&state=1234567890");
            builder.Append("&scope=user-read-private");
            builder.Append("&show_dialog=true");
            return builder.ToString();
        }

        private string GetRecommendationUri(RecommendationsModel recModel)
        {
            var criteria = recModel.criteria;
            var endpoint = new StringBuilder("recommendations");
            endpoint.Append($"?seed_artists={criteria.seed_artists}");
            endpoint.Append($"&seed_tracks={criteria.seed_tracks}");
            endpoint.Append($"&seed_genres={criteria.seed_genres}");
            endpoint.Append($"&limit={criteria.limit}");
            endpoint.Append($"&market={criteria.market}");

            endpoint.Append($"&target_danceability={criteria.target_danceability}");
            endpoint.Append($"&target_energy={criteria.target_energy}");
            endpoint.Append($"&target_popularity={criteria.target_popularity}");
            if (criteria.target_tempo > 0)
            {
                endpoint.Append($"&target_tempo={criteria.target_tempo}");
            }
            endpoint.Append($"&target_valence={criteria.target_valence}");

            return endpoint.ToString();
        }

        public async Task<T> GetSpotifyUserProfileAsync<T>()
        {
            string sEndPoint = $"me";

            T userInfo = await GetFromSpotifyAsync<T>(sEndPoint);

            return userInfo;
        }

        public async Task<T> GetUserPlaylistsAsync<T>(string userID)
        {
            string sEndPoint = $"users/{userID}/playlists";

            T rep = await GetFromSpotifyAsync<T>(sEndPoint);

            return rep;
        }

        public async Task<SpotifyRecommendations> GetRecommendationsAsync(RecommendationsModel criteria)
        {
            string endpoint = GetRecommendationUri(criteria);

            SpotifyRecommendations spotRecs = await GetFromSpotifyAsync<SpotifyRecommendations>(endpoint);

            return spotRecs;
        }


        /// <summary>
        /// Search for results in Spotify
        /// </summary>
        /// <param name="searchquery">Do not encode this string, will be done automatically</param>
        /// <param name="type"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<SpotifySearchResults> GetSearchResultsAsync(string searchquery, string type, int limit, int offset)
        {
            if (offset < 0) offset = 0;
            if (limit < 0) limit = 0;
            string endpoint = GetSearchUri(searchquery, type, limit, offset);
            var taskSearch = await GetFromSpotifyAsync<SpotifySearchResults>(endpoint);
            SpotifySearchResults spotResults = taskSearch;
            return spotResults;

        }

        /// <summary>
        /// Process a GET request and Deserialize the resulting return object.
        /// Not convinced that async and await are required in this function, simply Task<T> return value could be more efficient.
        /// </summary>
        /// <typeparam name="T">Any serializable type</typeparam>
        /// <param name="url_endpoint">Cannot start with a slash if a BaseAddress has been set in the HTTPClient.  A slash will bypass the BaseAddress</param>
        /// <returns></returns>
        public async Task<T> GetFromSpotifyAsync<T>(string url_endpoint)
        {
            SetHTTPClientHeaders();

            string res = await _httpClient.GetStringAsync(url_endpoint).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(res))
            {
                return default(T);
            }

            T newobject = JsonConvert.DeserializeObject<T>(res, _serialSettings);
            return newobject;
        }

    }
}