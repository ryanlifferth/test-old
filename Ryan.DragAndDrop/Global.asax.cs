using Ryan.DragAndDrop.Common;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Routing;

namespace Ryan.DragAndDrop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Add(new MyRazorViewEngine());
        }
    }
}
