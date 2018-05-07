using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class AboutController : Controller
    {
        // GET: Info
        public ActionResult Info() => View();

        public ActionResult OpeningTimes()
        {
            return View();
        }
        public ActionResult Librarian()
        {
            return View();
        }
    }
}   