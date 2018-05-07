using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using System.Globalization;

namespace UmbracoSpotify.Web.Models
{
    public class CallbackModel : RenderModel
    {
        public CallbackModel(IPublishedContent content, CultureInfo culture) : base(content,culture)
        {
        }

        public string accessToken { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string state { get; set; }
    }
}