using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ryan.WebAPI.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        // GET: About Us
        [ActionName("about-us")]
        public ActionResult About()
        {
            return View();
        }

    }
}