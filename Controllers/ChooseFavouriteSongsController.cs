using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using UmbracoSpotify.Spotify;
using UmbracoSpotify.Web.Models;

namespace UmbracoSpotify.Web.Controllers
{
    public class ChooseFavouriteSongsController : SurfaceController, IRenderController
    {
        private static SpotifyServices _spotService;
        private readonly AppSession _mySession;

        public ChooseFavouriteSongsController(SpotifyServices spot, SessionStateProvider baseSession) : base()
        {
            _spotService = spot;
            _mySession = new AppSession(baseSession);
               
        }

        private SearchCriteriaModel GetCriteriaClass(RenderModel model)
        {
            SearchCriteriaModel criteriaSpotify;
            if (_mySession.CSession["SearchCriteriaModel"] == null)
            {
                criteriaSpotify = new SearchCriteriaModel(model.Content, model.CurrentCulture);
                _mySession.CSession["SearchCriteriaModel"] = criteriaSpotify;
            }
            else
            {
                criteriaSpotify = (SearchCriteriaModel)_mySession.CSession["SearchCriteriaModel"];
            }
            return criteriaSpotify;
        }

        private bool AddFavouriteTrack(SearchCriteriaModel criteriaSpotify, string addtrackid, string addtrackname)
        {
            if (addtrackid != null && addtrackname != null)
                if (addtrackid != "" && addtrackname != "")
                {
                    if (!criteriaSpotify.FavouriteTracks.Exists(x => x.TrackID == addtrackid))
                    {
                        var newTrack = new FavouriteTrackItem();
                        newTrack.TrackID = addtrackid;
                        newTrack.TrackName = addtrackname;
                        criteriaSpotify.FavouriteTracks.Add(newTrack);
                    }
                }
            _mySession.CSession["SearchCriteriaModel"] = criteriaSpotify; // store in cache

            return true;
        }

        public async Task<ActionResult> AddFavouriteTrack(RenderModel model)
        {
            SpotifySearchResults res = (SpotifySearchResults)_mySession.CSession["SearchSearchResults"];
            ViewBag.SearchSpotify = res;
            return await Search(model);
        }

        public async new Task<ActionResult> Index(RenderModel model)
        {
            return await Search(model);
        }

        public async new Task<ActionResult> Search(RenderModel model)
        {
            SpotifySearchResults searchResults;

            var query = Request.QueryString;
            var cont = model.Content;

            SearchCriteriaModel criteriaSpotify = GetCriteriaClass(model);

            if (query.Count > 0)
            {
                criteriaSpotify.SearchText = query["searchText"];
                // Checkboxes may have entries duplicated in QueryString, but when true exists - they are Checked
                criteriaSpotify.ListAlbums = query["ListAlbums"].ToLower().Contains("true");
                criteriaSpotify.ListArtists = query["ListArtists"].ToLower().Contains("true");
                criteriaSpotify.ListPlaylist = query["ListPlaylist"].ToLower().Contains("true");
                criteriaSpotify.ListTrack = query["ListTrack"].ToLower().Contains("true");
                if (query["ShowPage"]!=null)
                    criteriaSpotify.PageNumber = int.Parse(query["ShowPage"]);

                if (query["addTrackid"] != null)
                {
                    // Just adding a new favourite, do not load more searches
                    AddFavouriteTrack(criteriaSpotify, query["addTrackid"], query["addTrackname"]);

                    // Remove any selected Favourite tracks from the display output
                    // as the User has already chosen them
                    searchResults = (SpotifySearchResults) _mySession.CSession["SearchSearchResults"];

                    searchResults.RemoveFavouriteTracks(criteriaSpotify.FavouriteTracks.Select(x => x.TrackID).ToList());

                    return View("ChooseFavouriteSongs", criteriaSpotify);
                }
            }

            // Wrong place for a default value of SearchText, perhaps best from Umbraco instead
            if (criteriaSpotify.SearchText == null)
                criteriaSpotify.SearchText = "eye of the tiger";

            if (ModelState.IsValid && criteriaSpotify.TypeParamString()!="")
            {
                if (criteriaSpotify.PageNumber <= 0)
                    criteriaSpotify.PageNumber = 1;

                searchResults = await  _spotService.GetSearchResultsAsync(criteriaSpotify.SearchText
                    , criteriaSpotify.TypeParamString(), criteriaSpotify.PageSize, (criteriaSpotify.PageNumber - 1) * criteriaSpotify.PageSize);

                criteriaSpotify.IsNextPage = false;
                if (searchResults != null)
                {
                    if (searchResults.albums == null ? false : searchResults.albums.next!=null)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.artists == null ? false : searchResults.artists.next != null)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.playlists == null ? false : searchResults.playlists.next != null)
                        criteriaSpotify.IsNextPage = true;
                    if (searchResults.tracks == null ? false : searchResults.tracks.next != null)
                        criteriaSpotify.IsNextPage = true;
                }

                // Remove any selected Favourite tracks from the display output
                // as the User has already chosen them
                searchResults.RemoveFavouriteTracks(criteriaSpotify.FavouriteTracks.Select(x => x.TrackID).ToList());

                _mySession.CSession["SearchSearchResults"] = searchResults;
                ViewBag.SearchSpotify = searchResults;
            }

            _mySession.CSession["SearchCriteriaModel"] = criteriaSpotify;
            return View("ChooseFavouriteSongs", criteriaSpotify);
        }


        //public ActionResult Pager(string searchText, bool? ListAlbums, bool? ListPlaylist, bool? ListArtists, bool? ListTrack, int? ShowPage)
        //[HttpGet]
        //public ActionResult Pager(string sURI)
        //{
        //    var query = Request.QueryString;

        //    SearchCriteriaModel criteriaSpotify = (SearchCriteriaModel)_mySession.CSession["SearchCriteriaModel"];

        //    if (query.Count > 0)
        //    {
        //        // Below, all these rows need to check for null in the query object
        //        criteriaSpotify.SearchText = query["searchText"];
        //        // Checkboxes may have entries duplicated in QueryString, but when true exists - they are Checked
        //        criteriaSpotify.ListAlbums = query["ListAlbums"].ToLower().Contains("true");
        //        criteriaSpotify.ListArtists = query["ListArtists"].ToLower().Contains("true");
        //        criteriaSpotify.ListPlaylist = query["ListPlaylist"].ToLower().Contains("true");
        //        criteriaSpotify.ListTrack = query["ListTrack"].ToLower().Contains("true");
        //        criteriaSpotify.PageNumber = int.Parse(query["ShowPage"]);
        //    }


        //    _mySession.CSession["SearchCriteriaModel"] = criteriaSpotify;
        //    return View("ChooseFavouriteSongs", criteriaSpotify);
        //}

    }
}