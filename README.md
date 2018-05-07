# UmbracoSpotify
Revision of Spotify Music recommending system using Umbraco for the presentation website

Notes:

(Umbraco login credentials available upon request)

- AutoFac package used for Dependency injection
- No exception handling has been added yet.  I would use Log4net for logging any exceptions, so that they can be examined later.  Also a dedicated landing page for exceptions that should be shown to the Web user (to some extent)

Requirements from CloudNine:

- The application should be built on top of Umbraco CMS. This is a free-to-use CMS, and we want all views, content (except for the content from Spotify), images and navigation to come from Umbraco rather than be hard-coded in the application.

- The functionality of the application should also be exposed through RESTful endpoints. These endpoints do not have to have any functionality, but we would like to see them set up with routes and be callable and return JSON with a message that just describes what the endpoint would do.

Apart from that, we would also have some general rules that the entire project should follow:

- Dependency injection should be used.
- A project structure that would make it easy for the application to grow and for parts of it to be exchangeable.
- Calls to external services should be asynchronous.
- Any caching or session handling should be treated in a way that allows for multiple users to use the application at once.
