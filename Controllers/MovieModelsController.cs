using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieShop.Models;
using MovieShop.Models.ViewModels;

namespace MovieShop.Controllers
{
    public class MovieModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: MovieModels
        public ActionResult NewUserSuccess()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }

        public ActionResult MoviesList()
        {
            return View(db.Movies.ToList());
        }

        public ActionResult AddToCart(int MovieId, int Add, int IsCart)
        {
            List<int> MovieIdList = new List<int>();
            if (Session["MoviesList"] != null) MovieIdList = (List<int>)Session["MoviesList"];

            if (Add == 0)
            {
                MovieIdList.Reverse();
                MovieIdList.Remove(MovieId);
                MovieIdList.Reverse();
            }
            else
            {
                MovieIdList.Add(MovieId);
            }

            Session["MoviesList"] = MovieIdList;
            Session["MoviesCount"] = MovieIdList.Count;
            if (IsCart == 0) return RedirectToAction("MoviesList");
            return RedirectToAction("DisplayCart", new {IsOrder = 0});
        }

        public ActionResult DisplayCart(int IsOrder)
        {
            Session["IsOrder"] = IsOrder;
            if (Session["MoviesList"] == null) return View("NoItems");
            List<int> SelectedMovIdList = new List<int>();
            List<int> DistinctMovIdList = new List<int>();
            List<MovieListVM> MoviesVMList = new List<MovieListVM>();
            MovieListVM vmobj;
            decimal total = 0;
            
            SelectedMovIdList = (List<int>)Session["MoviesList"];
            foreach (int id in SelectedMovIdList)
            {
                if (!DistinctMovIdList.Contains(id))
                {
                    DistinctMovIdList.Add(id);
                }
            }
            foreach (int Id in DistinctMovIdList)
            {
                vmobj = new MovieListVM();
                var movie = db.Movies.Find(Id);
                int copies = SelectedMovIdList.Count(m => m == Id);
                vmobj.MovieId = movie.Id;
                vmobj.Movie = movie.Title;
                vmobj.ReleaseYear = movie.ReleaseYear;
                vmobj.NoofCopies = copies;
                vmobj.Price = movie.Price * copies;
                
                total += vmobj.Price;
                MoviesVMList.Add(vmobj);
            }
            ViewBag.TotalPrice = total;
            return View(MoviesVMList);
        }

        // GET: MovieModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        // GET: MovieModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MovieModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,Price,ImageUrl")] MovieModel movieModel)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movieModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movieModel);
        }

        // GET: MovieModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        // POST: MovieModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,Price,ImageUrl")] MovieModel movieModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movieModel);
        }

        // GET: MovieModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        // POST: MovieModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieModel movieModel = db.Movies.Find(id);
            db.Movies.Remove(movieModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
