using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UmbracoSpotify.Web.Models
{
    public class ConnectModel: RenderModel
    {
        public ConnectModel(IPublishedContent content, CultureInfo culture) : base(content, culture)
        {
        }

        public string ConnectURI { get; set; }
    }
}