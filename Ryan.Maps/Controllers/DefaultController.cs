using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ryan.Maps.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        // GET: Google Map
        public ActionResult GoogleMap()
        {
            return View();
        }

        [ActionName("AddressArray-Google")]
        public ActionResult AddressArrayGoogle()
        {
            return View();
        }

        public ActionResult Buttons()
        {
            return View();
        }
    }
}