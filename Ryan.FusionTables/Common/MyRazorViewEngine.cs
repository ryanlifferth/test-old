using System.Linq;
using System.Web.Mvc;

namespace Ryan.FusionTables.Common
{
    /// <summary>
    /// Allows partial views to be put in a PartialViews folder under the Views\Controller folder.
    /// </summary>
    public class MyRazorViewEngine : RazorViewEngine
    {

        private static string[] NewPartialViewFormats = new[] {
                "~/Views/{1}/PartialViews/{0}.cshtml",
                "~/Views/Shared/PartialViews/{0}.cshtml"
        };

        public MyRazorViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
        }
    }
}