namespace Articulate
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// This create a route with a given virtual node route handler
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="defaults"></param>
        /// <param name="virtualNodeHandler"></param>
        /// <param name="constraints"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public static Route MapUmbracoRoute(this RouteCollection routes,
            string name, string url, object defaults, 
            UmbracoVirtualNodeRouteHandler virtualNodeHandler, 
            object constraints = null, string[] namespaces = null)
        {
            //We need to check if this route already exists, this will be the case when there is multi-tenency 
            // involved inside of umbraco. For example a root articulate node might yield a route like:
            // /tags/{tag}
            //and another articulate root node that has a domain might have this url:
            // http://mydomain/tags/{tag}
            // but when that is processed through RoutePathFromNodeUrl, it becomes:
            // /tags/{tag} which already exists and is already assigned to a specific node ID. So we need to deal with that
            // in a special way

            var route = RouteTable.Routes.MapRoute(name, url, defaults, constraints, namespaces);
            route.RouteHandler = virtualNodeHandler;
            return route;
        }

        /// <summary>
        /// Returns a route path from a given node's URL since a node's Url might contain a domain which we can't use in our routing.
        /// </summary>
        /// <param name="routePath"></param>
        /// <returns></returns>
        internal static string RoutePathFromNodeUrl(string routePath)
        {
            Uri result;
            return Uri.TryCreate(routePath, UriKind.Absolute, out result) 
                ? result.PathAndQuery.TrimStart('/')
                : routePath.TrimStart('/');
        }
    }
}