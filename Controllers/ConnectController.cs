using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using System.Globalization;

namespace UmbracoSpotify.Web.Controllers
{
    public class ConnectController : RenderMvcController
    {

        static SpotifyServices _spotService;

        public ConnectController(SpotifyServices spot)
        {
            _spotService = spot;
        }

        public override ActionResult Index(RenderModel model)
        {
            Models.ConnectModel conModel = new Models.ConnectModel(model.Content, CultureInfo.CurrentCulture)
            {
                ConnectURI = _spotService.GetAuthorizeUri()
            };

            return CurrentTemplate(conModel);
        }


        public ActionResult Callback(RenderModel model)
        {
            return CurrentTemplate(model);
        }

        public ActionResult Callback(string sURL)
        {
            return View();
        }

        public ActionResult Browse(RenderModel model)
        {
            return CurrentTemplate(model);
        }
    }
}