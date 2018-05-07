using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;
using UmbracoSpotify.Recommendations;
using UmbracoSpotify.Spotify;
using UmbracoSpotify.Web.Models;

namespace UmbracoSpotify.Web.Controllers
{
    /// <summary>
    /// All end points should use async / await when dealing with external APIs, Databases IO, etc (Any IO).  This
    /// will allow the Server to process more Requests without using more resources.  (Of course, async code is a tiny bit slower
    /// than synchronous code - but the benefits are necessary as the entire Web Server / Website / API performance will improve
    /// especially when scale increases)
    /// CPU Bound tasks would benefit from using extra Threads or using Parallel processing (not async await) - but care needs to be taken
    /// with the Server resources.  (At some point spawning extra threads can cripple the server, but that is extreme)
    /// ConfigureAwait(false) should also be used to allow for any thread use for async await calls (returns).
    /// </summary>
    [Route("api/[controller]")]
    public class TestApiController : UmbracoApiController
    {
        /// <summary>
        /// Could use a variety of attributes, for example:
        /// [Umbraco.Web.WebApi.MemberAuthorize(AllowType = "Retailers")]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ConnectToSpotify()
        {
            string sMsg = "Connected.  Returning 'token' here for reuse in the Header in next end points";
            return Request.CreateResponse(System.Net.HttpStatusCode.OK,  sMsg);
        }

        [HttpGet]
        public HttpResponseMessage GetSpotifyUserInfo(string token)
        {
            
            SpotifyUserInfo user = new SpotifyUserInfo();

            user.DisplayName = "Test user display name";
            user.UserID = "SpotUserID";

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, user);
        }

        /// <summary>
        /// Can use Get or Post.  Post might be easier with so many criteria values to choose from.
        /// </summary>
        /// <param name="recCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetSpotifyRecommendations(RecommendationCriteriaBase recCriteria)
        {

            SpotifyRecommendations recs = new SpotifyRecommendations();

            // currently returns an empty class
            // These calls should use async await to free up resources on the Server

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, recs);
        }

    }
}