using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using MovieShop.Models;

namespace MovieShop.Controllers
{
    public class CustomersController : Controller
    {
        public ApplicationDbContext MovieDB;
        public List<Movies> Movies;
        public List<Customers> Customers;

        public CustomersController()
        {
            MovieDB = new ApplicationDbContext();
            Movies = new List<Movies>();
            Customers = new List<Customers>();
        }

        //Load the Movie Shop
        public ActionResult MovieShop()
        {
            Movies = MovieDB.Movies.ToList();

            Session["ID"] = Guid.NewGuid().ToString();

            if (Movies == null)
            {
                return View();
            }
            else
            {
                return View(Movies);
            }
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = MovieDB.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                MovieDB.Customers.Add(customers);
                MovieDB.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customers);
        }

        public ActionResult Contact()
        {
            return View();
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = MovieDB.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }


        [HttpPost]
        public ActionResult ShoppingCart()
        {
            string ShoppingCartContent = Request.Form["ShoppingList"];
            string [] ShoppingCartItems = ShoppingCartContent.Split('!');
            string[][] ShoppingCartItem = new string[ShoppingCartItems.Length-1][];

            for (int i = 0; i < ShoppingCartItems.Length-1; i++)
            {
                ShoppingCartItem[i] = ShoppingCartItems[i].Split('+');
            }

            ViewBag.Collection = ShoppingCartItem;
            Session["ShoppingCart"] = ShoppingCartItem;
            return View();
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                MovieDB.Entry(customers).State = EntityState.Modified;
                MovieDB.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customers);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = MovieDB.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customers customers = MovieDB.Customers.Find(id);
            MovieDB.Customers.Remove(customers);
            MovieDB.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
