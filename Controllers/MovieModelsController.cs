using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MovieShop.Models;
using MovieShop.Models.ViewModels;

namespace MovieShop.Controllers
{
    [Authorize]
    public class MovieModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        

        // GET: MovieModels
        public ActionResult NewUserSuccess()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            return View(db.Movies.ToList());
        }
        [AllowAnonymous]
        public ActionResult FrontPage()
        {
            //List<int> TopIds = new List<int>();
            List<Movie> TopFiveList = new List<Movie>();
            var topmovs = db.Movies
                .Join(db.OrderRows, m => m.Id, r => r.MovieId, (m, r) => new { m, r })
                .Where(mr => mr.m.Id == mr.r.MovieId)
                .GroupBy(mr => mr.r.MovieId)
                .Select(mr => new
                {
                    Id = mr.Key,
                    Count = mr.Count()
                }).OrderByDescending(mr => mr.Count).ToList().Take(5);
            foreach (var mov in topmovs)
            {
                TopFiveList.Add(db.Movies.Find(mov.Id));
            }

            var MovieList = db.Movies.OrderBy(m => m.Title).ToList().Take(5);
            var Newest = db.Movies.OrderByDescending(m => m.ReleaseYear).ToList().Take(5);
            var Oldest = db.Movies.OrderBy(m => m.ReleaseYear).ToList().Take(5);
            var Cheapest = db.Movies.OrderBy(m => m.Price).ToList().Take(5);
            var MostExpensive = db.Orders.OrderByDescending(t => t.TotalPrice).ToList().Take(1);

            var DyrastOrder2 = db.Orders.OrderByDescending(o => o.TotalPrice).FirstOrDefault();
            var DyrastCustomer = db.Customers.Where(c => c.Id == DyrastOrder2.CustomerId).FirstOrDefault();
            var CustomerName = DyrastCustomer.FirstName + " " + DyrastCustomer.LastName;
            //var DyrCust = db.
            var WealthyCust = db.Customers.FirstOrDefault().FirstName.Where(c => c == 1).ToList();

            ViewBag.CustName = CustomerName;
            // Todo Person Who Ordered

            var tupleModel = new Tuple<List<Movie>, List<Movie>,List<Movie>, List<Movie>, List<Order>, List<Movie>>
                (MovieList.ToList(), Newest.ToList(),Oldest.ToList(), Cheapest.ToList(), MostExpensive.ToList(), TopFiveList.ToList());

            return View(tupleModel);
        }

        public ActionResult PagedIndex()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            return View(db.Movies.ToList());
        }
        [AllowAnonymous]
        public ActionResult MoviesList()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (Session["Search"] != null) Session["PrevSearch"] = Session["Search"];
            Session.Remove("Search");
            return View(db.Movies.ToList());
        }
        [AllowAnonymous]
        public ActionResult SearchMovies()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (Session["PrevSearch"] != null) Session["Search"] = Session["PrevSearch"];
            return View();
        }
        [AllowAnonymous]
        public ActionResult FilterMoviesList(string srctxt)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<Movie> movieslist = new List<Movie>();
            Session["Search"] = srctxt;

            if (String.IsNullOrEmpty(srctxt))
            {
                ViewBag.Flag = 1;
                ViewBag.Count = 0;
            }
            else
            {
                movieslist = db.Movies.Where(m => m.Title.Contains(srctxt)).ToList();
                ViewBag.Flag = 0;
                ViewBag.Count = movieslist.Count();
            }

            return View("MoviesList", movieslist);
        }
        [AllowAnonymous]
        public ActionResult PageinateMovies(int page=1)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            ViewBag.Url = TempData["Url"];
            if (Session["Search"] != null) Session["PrevSearch"] = Session["Search"];
            Session.Remove("Search");
            Session["CurrentPage"] = page;
            PaginateVM Pageobj = new PaginateVM();
            Pageobj.CurrentPage = page;
            Pageobj.MoviesPerPage = 6;
            Pageobj.Movies = CurrentPageMovies(Pageobj.CurrentPage, Pageobj.MoviesPerPage);
            Pageobj.MoviesPerRow = 3;
            Pageobj.PageCount = PaginateVM.TotalPages(db.Movies.Count(), Pageobj.MoviesPerPage);

            return View(Pageobj);
        }
        public IEnumerable<Movie> CurrentPageMovies(int CurrentPage,int CountPerPage)
        {
            int startcount = (CurrentPage - 1) * CountPerPage;
            var movieslist = db.Movies.OrderBy(m => m.Id);
            return movieslist.Skip(startcount).Take(CountPerPage);
        }
        [AllowAnonymous]
        public ActionResult AddToCart(int MovieId, int Add, int IsCart)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<int> MovieIdList = new List<int>();
            if (Session["MoviesList"] != null) MovieIdList = (List<int>)Session["MoviesList"];

            if (Add == 1) MovieIdList.Add(MovieId);
            else
            {
                MovieIdList.Reverse();
                MovieIdList.Remove(MovieId);
                MovieIdList.Reverse();
            }


            Session["MoviesList"] = MovieIdList;
            Session["MoviesCount"] = MovieIdList.Count;

            if (MovieIdList.Count == 0) return View("NoItems");

            if (IsCart == 0) return RedirectToAction("MoviesList");
            if (IsCart == 2) return RedirectToAction("SearchMovies");
            if (IsCart == 3)
            {
                //To fix scroll in paginate MovieList for 1st postback
                TempData["Url"] = "https://localhost:44303/MovieModels/PageinateMovies?page=" + Session["CurrentPage"];
                return RedirectToAction("PageinateMovies", "MovieModels", new { page = Session["CurrentPage"] });
            }
            return RedirectToAction("DisplayCart", new {IsSummary = 0});
        }
        [AllowAnonymous]
        public ActionResult DisplayCart(int IsSummary)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<int> NoofCopiesList = new List<int>();
            List<decimal> PriceList = new List<decimal>();

            Session["IsSummary"] = IsSummary;
            if (Session["MoviesList"] == null || (int)Session["MoviesCount"] == 0) return View("NoItems");
            
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

                NoofCopiesList.Add(vmobj.NoofCopies);
                PriceList.Add(vmobj.Price);
                MoviesVMList.Add(vmobj);
            }
            ViewBag.TotalPrice = total;
            Session["MovieIdList"] = DistinctMovIdList;
            Session["CopiesList"] = NoofCopiesList;
            Session["PriceList"] = PriceList;
            Session["TotalPrice"] = total;
            return View(MoviesVMList);
        }
        [AllowAnonymous]
        public ActionResult SubmitOrder()
        {
            List<int> MovieIdList = new List<int>();
            List<int> CopiesList = new List<int>();
            List<decimal> PriceList = new List<decimal>();
            int idx = 0;

            Order ord = new Order();
            ord.CustomerId = (int)Session["CustId"];
            ord.OrderDate = DateTime.Now;
            ord.TotalPrice = (decimal)Session["TotalPrice"];
            db.Orders.Add(ord);
            db.SaveChanges();

            MovieIdList = (List<int>)Session["MovieIdList"];
            CopiesList  = (List<int>)Session["CopiesList"];
            PriceList   = (List<decimal>)Session["PriceList"];

            foreach (var movieid in MovieIdList)
            {
                OrderRow ordrw = new OrderRow();
                ordrw.OrderId = ord.Id;
                ordrw.MovieId = movieid;
                ordrw.NoofCopies = CopiesList.ElementAt(idx);
                ordrw.Price = PriceList.ElementAt(idx);

                db.OrderRows.Add(ordrw);
                db.SaveChanges();

                idx += 1;
            }
            ViewBag.OrderId = ord.Id;
            Session.Clear();
            //Session.Remove("") clears single variable
            return View();
        }

        // GET: MovieModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movieModel = db.Movies.Find(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,Price,ImageUrl")] Movie movieModel)
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
            Movie movieModel = db.Movies.Find(id);
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
        public ActionResult Edit([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,Price,ImageUrl")] Movie movieModel)
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
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movieModel = db.Movies.Find(id);
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
            Movie movieModel = db.Movies.Find(id);
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
