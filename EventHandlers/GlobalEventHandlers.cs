using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Reflection;
using System;
using System.Web;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Core.Services;
using System.Web.Mvc;
using System.Web.Http;
using System.Net.Http;

namespace UmbracoSpotify.EventHandlers
{
    public class GlobalEventHandlers : IApplicationEventHandler
    {
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(ApplicationContext.Current).AsSelf();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);

            RegisterTypes(builder, applicationContext);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }

        private void RegisterTypes(ContainerBuilder builder, ApplicationContext applicationContext)
        {
            builder.RegisterInstance(applicationContext.Services.MemberService).As<IMemberService>();

            // SessionStateProvider for (User) Session objects, which should be provided
            // as a singleton as per its behaviour
            builder.RegisterType<SessionStateProvider>().SingleInstance();

            // SpotifyServices class, quite simple - can make new instances ad hoc
            builder.RegisterType<SpotifyServices>().AsSelf();

            // Single instance, which provides the start up 
            // properties for HttpClient within, and should Resolve a 
            // singleton of HttpClient
            builder.RegisterType<HttpSpotify>().SingleInstance();


            // HTTPClient object, which we want to remain static for the entire Application lifecycle 
            // Although really we should be using more than one HTTPClient, where each instance
            // has its unique Header values...(Of which there will be 2 for Spotify...One before we 
            // have the access_token, and the other when we do use the access_token in the Header) ? TODO check
            // We could use .InstancePerMatchingLifetimeScope("myrequest");  but this is not implemented yet

            // Mind you, this is now superfluous as the HttpSpotify class
            // controls the start up values for HttpClient and only a SingleInstance
            // is going to be Resolved of HttpSpotify.  So this Registration below
            // can be modified to suit
            builder.RegisterType<HttpClient>().SingleInstance();

        }

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }


    }
}