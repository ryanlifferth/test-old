﻿using System;
using Ryan.AngularJS.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Ryan.AngularJS.App_Start;
using System.Web.Optimization;

namespace Ryan.AngularJS
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Add(new MyRazorViewEngine());
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}