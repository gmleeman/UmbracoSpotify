using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using UmbracoSpotify.Recommendations;

namespace UmbracoSpotify.Web.Models
{
    public class RecommendationsModel : RenderModel
    {
        public RecommendationCriteriaBase criteria { get; set; }
        public RecommendationsModel(IPublishedContent content, CultureInfo culture) : base(content, culture)
        {
            criteria = new RecommendationCriteriaBase();
        }

        //public int limit { get; set; }
        //public string market { get; set; }
        //public string seed_genres { get; set; }
        //public string seed_artists { get; set; }
        //public string seed_tracks { get; set; }
        //public float target_energy { get; set; }
        //public float target_danceability { get; set; }
        //public float target_tempo { get; set; }
        //public float target_valence { get; set; }
        //public int target_popularity { get; set; }
    }
}