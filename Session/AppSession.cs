using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace UmbracoSpotify
{
    public interface ISessionStateProvider
    {
        object this[string key] { get; set; }
        void Remove(string key);
    }

    public class SessionStateProvider : ISessionStateProvider
    {
        public object this[string key]
        {
            get
            {
                return HttpContext.Current.Session[key];
            }
            set
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        public void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }
    }

    public class SessionBaseProvider
    {
        public SessionBaseProvider(ISessionStateProvider sessionObject)
        {
            CSession = sessionObject;
        }
        public ISessionStateProvider CSession { get; set; }
    }

    public class AppSession : SessionBaseProvider
    {
        public AppSession(ISessionStateProvider sessionObject) : base(sessionObject)
        {

        }
    }
}