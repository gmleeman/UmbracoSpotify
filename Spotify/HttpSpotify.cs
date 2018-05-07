using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace UmbracoSpotify
{
    /// <summary>
    /// Holds an instance of HttpClient and sets the initial
    /// properties BaseAddress and Timeout
    /// </summary>
    public class HttpSpotify
    {
        /// <summary>
        /// This base URL MUST end with a slash, and any end points
        /// subsequently cannot start with a slash
        /// </summary>
        private string _BaseURL = "https://api.spotify.com/v1/";

        public HttpSpotify(HttpClient client)
        {
            SpotifyClient = client;
            SpotifyClient.BaseAddress = new Uri(_BaseURL);
            SpotifyClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public HttpClient SpotifyClient { get; private set; }
    }
}