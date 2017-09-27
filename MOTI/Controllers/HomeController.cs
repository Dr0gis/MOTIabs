using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MOTI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

        public ActionResult CreateAlternative()
        {
            ViewBag.Message = "Add an alternative";

            return View();
        }

        public ActionResult ListAlternative()
        {
            ViewBag.Message = "List an alternative";
            Database1Entities _entities = new Database1Entities();
            DbSet<Alternative> model = _entities.Alternative;

            return View(model);
        }
    }
}