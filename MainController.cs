using ServicesForHire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using IdentitySample.Models;

namespace ServicesForHire.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            //var product = (from d in db.Products select d).ToList();
            //return View(product);
            TempData["CartHire"] = new List<Service>();

            var result = db.services.Include(s => s.Categories).ToList();
            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Details(int id)
        {
            var product = db.services.Find(id);

            return View(product);
        }


        //public ActionResult Index2()
        //{
        //    var categ = db.Categories.ToList();

        //    return View(categ);
        //}

       
        public ActionResult Browse(string categorypar)
        {
            var cat_model = db.Categories.Include("Services").Single(g => g.categoryName == categorypar);
            return View(cat_model);
        }

        public ActionResult GetSearchRecord(string search)
        {
            var check = from x in db.services
                        select x;

            check = check.Where(x => x.name.Contains(search) || x.productDescription.Contains(search)
             || x.Categories.categoryName.Contains(search));

          
             return View(check.OrderBy(x => x.name.ToString()));
        }


    }
}