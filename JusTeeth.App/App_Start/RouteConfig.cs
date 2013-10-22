﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JusTeeth.App.Controllers;

namespace JusTeeth.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UserPicture",
                url: "Users/GetUserPicture",
                defaults: new { controller = "Users", action = "GetUserPicture" }
            );

            routes.MapRoute(
                name: "UserEdit",
                url: "Users/EditProfile",
                defaults: new { controller = "Users", action = "EditProfile" }
            );

            routes.MapRoute(
                name: "Users",
                url: "Users/{username}",
                defaults: new {  controller = "Users", action = "Index" }
                );

            routes.MapRoute(
                name: "UserFriends",
                url: "Users/Friend/{username}",
                defaults: new { controller = "Users", action = "Friend" }
                );

            routes.MapRoute(
                name: "Places",
                url: "Places/{action}/{id}",
                defaults: new {controller = "Places", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "JusTeeth.App.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                
            );
        }
    }
}
