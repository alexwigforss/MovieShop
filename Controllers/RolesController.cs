using MovieShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieShop.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserRolesList()
        {
            ViewBag.Roles = db.Roles.ToList();
            return View();
        }
    }
}