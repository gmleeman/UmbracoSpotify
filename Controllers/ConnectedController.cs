using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using UmbracoSpotify.Web.Models;
using UmbracoSpotify.Spotify;

namespace UmbracoSpotify.Web.Controllers
{
    public class ConnectedController : RenderMvcController
    {
        static SpotifyServices _spotService;
        private readonly AppSession _mySession;

        public ConnectedController(SpotifyServices spot, SessionStateProvider baseSession) :base()
        {
            _spotService = spot;
            _mySession = new AppSession(baseSession);
        }

        public async Task<ActionResult> Index(RenderModel model)
        {
            SpotifyUserInfoModel conModel = new SpotifyUserInfoModel(model.Content, CultureInfo.CurrentCulture);

            int iCount = await _spotService.LoadSeedGenresAsync();

            conModel.info = await _spotService.GetSpotifyUserProfileAsync<SpotifyUserInfo>();
            
            SpotifyUserPlaylist playlists =  await _spotService.GetUserPlaylistsAsync<SpotifyUserPlaylist>(conModel.info.UserID);
            conModel.playlists = playlists;

            return CurrentTemplate(conModel);
        }
    }
}