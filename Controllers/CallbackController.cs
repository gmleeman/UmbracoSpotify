using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using System.Globalization;
using Umbraco.Core.Models;

namespace UmbracoSpotify.Web.Controllers
{
    public class CallbackController : SurfaceController
    {
        private readonly AppSession _mySession;
        public CallbackController(SessionStateProvider baseSession) : base()
        {
            _mySession = new AppSession(baseSession);
        }

        public ActionResult Index(RenderModel model)
        {
            Models.CallbackModel conModel = new Models.CallbackModel(model.Content, CultureInfo.CurrentCulture)
            {
            };

            return View(conModel);
        }


        public ActionResult Callback(RenderModel model, string access_token, string token_type, string expires_in, string state)
        {
            if (string.IsNullOrEmpty(access_token))
                return View();

            Models.CallbackModel conModel = new Models.CallbackModel(model.Content, CultureInfo.CurrentCulture)
            {
                accessToken = access_token,
                token_type = token_type,
                expires_in = expires_in,
                state = state
            };

            _mySession.CSession["SpotifyAccess"] = conModel;
            var nodes = umbraco.uQuery.GetNodesByType("Connected");
            if (nodes.Any())
            {
                IPublishedContent node = Umbraco.TypedContent(nodes.First().Id);
                return RedirectToUmbracoPage(node.Id);
            }
            return this.Redirect("Connected");
        }

    }
}