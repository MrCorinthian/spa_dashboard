using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Threading;
using System.Globalization;
using System.Net.Http.Headers;

namespace WebApplication13
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
