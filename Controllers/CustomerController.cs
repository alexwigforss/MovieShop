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
    public class CustomerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customer
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        public JsonResult FamousMovie()
        {
            List<int> TopIds = new List<int>();
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
            foreach (var obj in TopFiveList)
            {
                TopIds.Add(obj.Id);
            }

            // var mov=db.Movies.Find(mov.FirstOrDefault().Id)
            return Json(topmovs, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CheckOrders(string email)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<OrderHistoryVM> OrdVmList = new List<OrderHistoryVM>();
            if (!String.IsNullOrEmpty(email))
            {
                Customer Cust = db.Customers.Where(c => c.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
                if (Cust != null)
                {
                    OrdVmList=db.Orders.Where(o => o.CustomerId == Cust.Id)
                        .Join(db.OrderRows, o => o.Id, r => r.OrderId, (o, r) => new { o, r })
                        .Select(ord => new OrderHistoryVM
                        {
                            OrderId = ord.o.Id,
                            CustomerName=ord.o.Customer.FirstName+" "+ord.o.Customer.LastName,
                            MovieTitle=ord.r.Movie.Title,
                            OrderDate=ord.o.OrderDate,
                            Price=ord.r.Price,
                            TotalPrice=ord.o.TotalPrice,
                            NoofCopies=ord.r.NoofCopies
                        }).ToList();
                    //var temp = (from Orders in db.Orders.Where(o => o.CustomerId == Cust.Id) select m).ToList();
                }
            }
            return View(OrdVmList);
        }

        public ActionResult CheckOrdersforPV(string email)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<OrderHistoryPVVM> OrdVmLst = new List<OrderHistoryPVVM>();
                if (!String.IsNullOrEmpty(email))
                {
                    Customer Cust = db.Customers.Where(c => c.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
                ViewBag.CustomerName = Cust.FirstName + " " + Cust.LastName;
                if (Cust != null)
                    {
                        IEnumerable<Order> OrdersList = new List<Order>();
                        OrdersList = db.Orders.Where(o => o.CustomerId == Cust.Id).ToList();
                        foreach (var orderid in OrdersList.Select(o => o.Id).Distinct().ToList())
                        {
                            OrderHistoryPVVM vmobj = new OrderHistoryPVVM();
                            List<OrderHistoryListVM> vmlistobj = new List<OrderHistoryListVM>();
                            vmlistobj = db.Orders.Where(o => o.CustomerId == Cust.Id && o.Id == orderid)
                                 .Join(db.OrderRows, o => o.Id, r => r.OrderId, (o, r) => new { o, r })
                                 .Select(ord => new OrderHistoryListVM
                                 {
                                     MovieTitle = ord.r.Movie.Title,
                                     Price = ord.r.Price,
                                     NoofCopies = ord.r.NoofCopies
                                 }).ToList();
                            vmobj.OrderId = orderid;
                            vmobj.OrderDate = OrdersList.Where(o => o.Id == orderid).Select(o => o.OrderDate).FirstOrDefault();
                            vmobj.TotalPrice = OrdersList.Where(o => o.Id == orderid).Select(o => o.TotalPrice).FirstOrDefault();
                            vmobj.OrderHistoryList = vmlistobj;
                            OrdVmLst.Add(vmobj);
                        }
                    }
                }
                return View(OrdVmLst);
            
        }

        public ActionResult ValidateCustomer(string email)
        {
            Session["Msg"] = "Already a Customer!";
            Session["Valid"] = 1;
            Customer cust = db.Customers.Where(c => c.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
            if (cust == null)
            {
                Session["Msg"] = "Not a registered Customer!";
                Session["Valid"] = 0;
            }
            else
            {
                Session["CustId"] = cust.Id;
            }
            Session["UsrEmail"] = email;
            ViewBag.Email = email;
            return View();
        }

        public ActionResult NewUserSuccess()
        {
            return View();
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customerModel = db.Customers.Find(id);
            if (customerModel == null)
            {
                return HttpNotFound();
            }
            return View(customerModel);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        //public ActionResult TestView()
        //{
        //    return View("NewUserSuccess");
        //}

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customer customerModel)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customerModel);
                db.SaveChanges();
                Session["CustId"] = customerModel.Id;
                //return RedirectToAction("NewUserSuccess");
                ViewBag.Email = customerModel.EmailAddress;
                return View("NewUserSuccess");
                //return RedirectToAction("DisplayCart");
            }
            
            return View(customerModel);
            //return View(customerModel);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customerModel = db.Customers.Find(id);
            if (customerModel == null)
            {
                return HttpNotFound();
            }
            return View(customerModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customer customerModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerModel);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customerModel = db.Customers.Find(id);
            if (customerModel == null)
            {
                return HttpNotFound();
            }
            return View(customerModel);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customerModel = db.Customers.Find(id);
            db.Customers.Remove(customerModel);
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
