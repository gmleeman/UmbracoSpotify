using Newtonsoft.Json;
using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using UmbracoSpotify.Spotify;

namespace UmbracoSpotify.Web.Models
{
    public class SpotifyUserInfoModel : RenderModel
    {

        public SpotifyUserInfoModel(IPublishedContent content, CultureInfo culture) : base(content, culture)
        {
        }

        public SpotifyUserInfo info { get; set; }
        public SpotifyUserPlaylist playlists { get; set; }
    }
}