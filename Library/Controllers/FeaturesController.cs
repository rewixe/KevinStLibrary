using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class FeaturesController : Controller
    {
        // GET: Features
        public ActionResult Features()
        {
            return View();
        }
        public ActionResult BookSuggestions()
        {
            return View();
        }
    }
}