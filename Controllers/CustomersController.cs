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
        public List<Orders> Orders;
        public List<OrderRows> OrderRows;

        public CustomersController()
        {
            MovieDB = new ApplicationDbContext();
            Movies = new List<Movies>();
            Customers = new List<Customers>();
            Orders = new List<Orders>();
            OrderRows = new List<OrderRows>();
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


        public ActionResult Contact()
        {
            return View();
        }

        internal string [][] ShoppingList(string shoppinglist)
        {
            string[] ShoppingCartItems = shoppinglist.Split('|');
            string[][] ShoppingCartItem = new string[ShoppingCartItems.Length - 1][];

            for (int i = 0; i < ShoppingCartItems.Length - 1; i++)
            {
                ShoppingCartItem[i] = ShoppingCartItems[i].Split('+');
            }

            return ShoppingCartItem;
        }

        [HttpPost]
        public ActionResult PlaceOrder()
        {
            //// Get the form data from the form
            Customers customer = new Customers()
            {
                FirstName = Request.Form["FirstName"],
                LastName = Request.Form["LastName"],
                EmailAddress = Request.Form["Email"],
                PhoneNo = Request.Form["Phone"],
                BillingAddress = Request.Form["BillingAdress"],
                BillingZip = Request.Form["BillingZip"],
                BillingCity = Request.Form["BillingCity"],
                DeliveryAddress = (String.IsNullOrEmpty(Request.Form["ShippingAdress"])) ? Request.Form["BillingAdress"] : Request.Form["ShippingAdress"],
                DeliveryZip = (String.IsNullOrEmpty(Request.Form["ShippingZip"])) ? Request.Form["BillingZip"] : Request.Form["ShippingZip"],
                DeliveryCity = (String.IsNullOrEmpty(Request.Form["ShippingCity"])) ? Request.Form["BillingCity"] : Request.Form["ShippingCity"]
            };

            Boolean CreateAccount = Convert.ToBoolean(Request.Form["CreateAccountCheckBox"]);
           

            //// Insert Customer data in the database, if the customer does not exist

            ViewBag.Data = customer.FirstName + " " + customer.LastName + " " + customer.EmailAddress + " " +
                customer.PhoneNo + " " + customer.BillingAddress + " " + customer.BillingZip + " " +
                customer.BillingCity + " " + customer.DeliveryAddress + " " + customer.DeliveryZip + " " +
                customer.DeliveryCity + " " + CreateAccount;

            Session["ShoppingCart"] = ShoppingList(Request.Form["FinalShoppingList"]);
            ViewBag.FinalShoppingList = Session["ShoppingCart"];

            //Check whether User has already an account in AspNetUsers
            if (CreateAccount)
            {
                
            }

            // Check whether user is already in Customers Table
            var IsAlreadyCustomer = MovieDB.Customers.Where(c => c.FirstName == customer.FirstName &&
            c.LastName == customer.LastName && c.EmailAddress == customer.EmailAddress &&
            c.PhoneNo == customer.PhoneNo && c.BillingAddress == customer.BillingAddress &&
            c.BillingZip == customer.BillingZip);

            if (!IsAlreadyCustomer.Any())
            {
                MovieDB.Customers.Add(customer);
                MovieDB.SaveChanges();
            }

            //// Create Order in the database

            //// Load a page with the Order Details

            return View();
        }






        // ------------------------------- Microsoft generated methods ------------------------//
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
