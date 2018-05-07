using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using UmbracoSpotify.Web.Models;

namespace UmbracoSpotify.Controllers
{
    public class RecommendationsController : SurfaceController, IRenderController
    {
        private static SpotifyServices _spotService;
        private readonly AppSession _mySession;

        public RecommendationsController(SpotifyServices spot, SessionStateProvider baseSession) : base()
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

        public async Task<ActionResult> Index(RenderModel model)
        {
            var newRecSearch = new RecommendationsModel(model.Content, model.CurrentCulture);
            SearchCriteriaModel criteriaSpotify = GetCriteriaClass(model);

            newRecSearch.criteria.seed_tracks = string.Join(",", criteriaSpotify.FavouriteTracks.Select(x => x.TrackID).ToList());
            newRecSearch.criteria.seed_artists = "";
            newRecSearch.criteria.market = "SE";
            newRecSearch.criteria.limit = 50;

            var query = Request.QueryString;
            if (query != null && query.Count>0)
            {
                criteriaSpotify.target_popularity = int.Parse(query["target_popularity"]);
                criteriaSpotify.target_danceability = Single.Parse(query["target_danceability"]);
                criteriaSpotify.target_energy = Single.Parse(query["target_energy"]);
                criteriaSpotify.target_tempo = int.Parse(query["target_tempo"]);
                criteriaSpotify.target_valence = Single.Parse(query["target_valence"]);
                criteriaSpotify.seed_genres = query["seed_genres"].ToString();
            }
            _mySession.CSession["SearchCriteriaModel"] = criteriaSpotify;

            newRecSearch.criteria.seed_genres = criteriaSpotify.seed_genres;
            newRecSearch.criteria.target_popularity = criteriaSpotify.target_popularity;
            newRecSearch.criteria.target_danceability = criteriaSpotify.target_danceability;
            newRecSearch.criteria.target_energy = criteriaSpotify.target_energy;
            newRecSearch.criteria.target_tempo = criteriaSpotify.target_tempo;
            newRecSearch.criteria.target_valence = criteriaSpotify.target_valence;

            var recResults = await _spotService.GetRecommendationsAsync(newRecSearch);

            ViewBag.Recommendations = recResults;
            ViewBag.GenreList = _spotService.genres;

            return View("Recommendations", criteriaSpotify);
        }
    }
}