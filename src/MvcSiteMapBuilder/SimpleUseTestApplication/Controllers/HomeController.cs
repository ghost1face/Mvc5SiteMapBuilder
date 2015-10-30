using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleUseTestApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            MvcSiteMapBuilder.SiteMaps.GetSiteMap();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}