﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using MovieShop.Models;
using System.Drawing;
using System.IO;
using System.Web.Helpers;
using PagedList;
using System.Net;
using System.Web.Services;
using System.Web.Script.Services;
using System.Configuration;

namespace MovieShop.Controllers
{
    public class AllowedIPAttribute : ActionFilterAttribute
    {

        //overrinding OnActionExecuting method to check Ip, before any code from Action is executed.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Retrieve user's IP
            string usersIpAddress = HttpContext.Current.Request.UserHostAddress;

            if (!checkIp(usersIpAddress))
            {
                //return 403 Forbidden HTTP code
                filterContext.Result = new HttpStatusCodeResult(404);
            }

            base.OnActionExecuting(filterContext);
        }



        public static bool checkIp(string usersIpAddress)
        {
            //get allowedIps Setting from Web.Config file and remove whitespaces from int
            string allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();


            //convert allowedIPs string to table by exploding string with ';' delimiter
            string[] ips = allowedIps.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //iterate ips table
            foreach (var ip in ips)
            {
                if (ip.Equals(usersIpAddress))
                    return true; //return true confirming that user's address is allowed
            }

            //if we get to this point, that means that user's address is not allowed, therefore returning false
            return false;

        }
    }
    public class AdminController : Controller
    {
        public ApplicationDbContext MovieDB;
        public List<Movies> Movies;
        public List<Customers> Customers;
        public List<Orders> Orders;
        public List<OrderRows> OrderRows;
        OrderDetails orderDetail;

        public AdminController()
        {
            MovieDB = new ApplicationDbContext();
            Movies = new List<Movies>();
            Customers = new List<Customers>();
            Orders = new List<Orders>();
            OrderRows = new List<OrderRows>();
            orderDetail = new OrderDetails();
        }

        // GET: Admin
        [AllowedIP]
        public ActionResult AdminLoginPage()
        {
            // If login Failed inform the user
            // if login succeeded write a welcome page and add a logout menu item to the navigation bar.
            return View();
        }

        //------------------------------------------------------------------------------------------------//
        //----------------------------- Movie Admin Page ----------------------------------------------//
        //------------------------------------------------------------------------------------------------//
        [AllowedIP]
        public ActionResult MovieAdminPage()
        {
            Movies = MovieDB.Movies.OrderBy(m => m.Id).ToList();

            if (Movies == null)
            {
                return View();
            }
            else
            {
                return View(Movies);
            }
        }

        [AllowedIP]
        public ActionResult CreateMovieAdminPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateMovie(Movies movie)
        {
            Movies Movie;

            MovieDB.Movies.Add(Movie = new Movies()
            {
                Title = movie.Title,
                Director = movie.Director,
                RealYear = movie.RealYear,
                Price = movie.Price,
                Url = movie.Url,
                ImgFile = WebImage.GetImageFromRequest(),
            }
             );

            Movie.Image = Movie.ImgFile.GetBytes();
            MovieDB.SaveChanges();

            return RedirectToAction("MovieAdminPage");
        }

        [AllowedIP]
        public ActionResult DeleteMovieAdminPage(int Id)
        {
            var movie = MovieDB.Movies.FirstOrDefault(m => m.Id == Id);

            if (movie == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(movie);
            }
        }

        [HttpPost]
        public ActionResult DeleteMovieAdminPage(Movies movie)
        {
            var Movie = MovieDB.Movies.FirstOrDefault(m => m.Id == movie.Id);

            if (Movie == null)
            {
                return HttpNotFound();
            }
            else
            {
                MovieDB.Movies.Remove(Movie);
                MovieDB.SaveChanges();
                return RedirectToAction("MovieAdminPage");
            }
        }

        [AllowedIP]
        public ActionResult ModifyMovieAdminPage(int Id)
        {
            var movie = MovieDB.Movies.FirstOrDefault(m => m.Id == Id);

            if (movie == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(movie);
            }
        }

        [HttpPost]
        public ActionResult ModifyMovieAdminPage(Movies movie)
        {
            var Movie = new Movies();
            Movie = MovieDB.Movies.Find(movie.Id);

            if (Movie == null)
            {
                return HttpNotFound();
            }
            else
            {
                Movie.Title = movie.Title;
                Movie.Director = movie.Director;
                Movie.RealYear = movie.RealYear;
                Movie.Price = movie.Price;
                Movie.Url = movie.Url;
                Movie.Image = movie.Image;

                MovieDB.Entry(Movie).State = System.Data.Entity.EntityState.Modified;
                MovieDB.SaveChanges();

                return RedirectToAction("MovieAdminPage");
            }
        }

        //------------------------------------------------------------------------------------------------//
        //----------------------------- Customer Admin Page ----------------------------------------------//
        //------------------------------------------------------------------------------------------------//
        [AllowedIP]
        public ActionResult CustomerAdminPage(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Customers = MovieDB.Customers.OrderBy(c => c.LastName).ToList();

            decimal numberofRows = Customers.Count;
            decimal pageSize = 5;
            int numberOfPages = (int)Math.Ceiling(numberofRows / pageSize);

            //int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(Customers.ToPagedList(pageNumber, (int)pageSize));
        }

        // ------------------------------- Microsoft generated methods ------------------------//
        // GET: Customers/Create
        [AllowedIP]
        public ActionResult CreateCustomer()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCustomer([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                MovieDB.Customers.Add(customers);
                MovieDB.SaveChanges();
                return RedirectToAction("CustomerAdminPage");
            }

            return View(customers);
        }


        // GET: Customers/Edit/5
        [AllowedIP]
        public ActionResult EditCustomer(int? id)
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
        public ActionResult EditCustomer([Bind(Include = "Id,FirstName,LastName,BillingAddress,BillingZip,BillingCity,DeliveryAddress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                MovieDB.Entry(customers).State = EntityState.Modified;
                MovieDB.SaveChanges();
                return RedirectToAction("CustomerAdminPage");
            }
            return View(customers);
        }

        // GET: Customers/Delete/5
        [AllowedIP]
        public ActionResult DeleteCustomer(int? id)
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
        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCustomerConfirmed(int id)
        {
            string forceDelete = Request.Form["force-delete-customer"];
            Customers Customer = MovieDB.Customers.Find(id);

            if (Customer != null)
            {
                // Find all the orders by the customer
                var orders = MovieDB.Orders.Where(o => o.CustomerId == Customer.Id).ToList();

                if (orders.Count != 0)
                {

                    // Remove all entries in OrderRow Table
                    foreach (var order in orders)
                    {
                        var orderRows = MovieDB.OrderRows.Where(or => or.OrderId == order.Id).ToList();

                        if (orderRows.Count != 0)
                        {
                            foreach (var orderRow in orderRows)
                            {
                                MovieDB.OrderRows.Remove(orderRow);
                            }
                        }
                        else
                        {
                            if (forceDelete != "Checked")
                            {
                                throw new Exception("Customers order cannot be deleted! Deletion aborted");
                            }
                        }
                    }

                    MovieDB.SaveChanges();
                }
                else
                {
                    if (forceDelete != "Checked")
                    {
                        throw new Exception("Customers order cannot be deleted! Deletion aborted");
                    }
                }
                // Remove all entries in Order Table
                foreach (var order in orders)
                {
                    MovieDB.Orders.Remove(order);
                }

                // Remove the customer
                MovieDB.Customers.Remove(Customer);
                MovieDB.SaveChanges();
                return RedirectToAction("CustomerAdminPage");
            }
            else
            {
                throw new Exception("Cannot remove the customer!");
            }
        }

        //------------------------------------------------------------------------------------------------//
        //----------------------------------------- Order Admin ------------------------------------------//
        //------------------------------------------------------------------------------------------------//
        [AllowedIP]
        public ActionResult OrderAdminPage()
        {
            Orders = MovieDB.Orders.OrderBy(c => c.Id).ToList();

            if (Session["OrderDetails"] != null)
            {
                ViewBag.OrderData = Session["OrderDetails"];
            }
                return View(Orders);
        }

        [HttpPost, ActionName("OrderAdminPage")]
        public ActionResult GetOrderDetails()
        {
            int id = Convert.ToInt32(Request.Form["index"]);
            string operation = "";
            string[] data = new string[2];
            if (id != 0) // To Show the order details
            {
                Orders order = MovieDB.Orders.Find(id);
                var customer = MovieDB.Customers.Where(c => c.Id == order.CustomerId).ToList();
                var orderRows = MovieDB.OrderRows.Where(o => o.OrderId == id).ToList();
                orderDetail.order = order;
                orderDetail.customer = customer[0];
                foreach (OrderRows orderRow in orderRows)
                {
                    var movie = (Movies)MovieDB.Movies.Find(orderRow.MovieId);
                    orderDetail.shoppingCart.Add(new OrderDetails.ShoppingCart()
                    {
                        qty = (int)(orderRow.Price / (double)movie.Price),
                        movieId = movie.Id,
                        movieTitle = movie.Title,
                        price = (double)movie.Price,
                        sum = orderRow.Price
                    });
                }

                foreach (var sc in orderDetail.shoppingCart)
                {
                    orderDetail.totalSum += sc.sum;
                }

                orderDetail.vat = orderDetail.totalSum * 0.2;


                Session["OrderDetails"] = orderDetail;

            }
            else
            {
                operation = Request.Form["OperationPlusOrderid"];

                if (!String.IsNullOrEmpty(operation))
                {
                    data = operation.Split(':');

                    if (data[0] == "edit")
                    {
                        OrderEdit(data[1]);
                    }
                    else if (data[0] == "delete")
                    {
                        OrderDelete(Convert.ToInt32(data[1]));
                    }
                }

                Session["OrderDetails"] = null;
            }


            return RedirectToAction("OrderAdminPage");
        }

        // Writes changes to customer data and customer order to the database.
        // Optional: an email shall be sent out. (Not implemented)
        private void OrderEdit(string data)
        {
            string[] customerAndOrderInfo = data.Split('|');
            string[] customerData = customerAndOrderInfo[0].Split('-');
            string[]shoppingData = customerAndOrderInfo[1].Split(';');
            string[][] shoppinglist = new string[shoppingData.Length - 1][];

            int orderId = Convert.ToInt32(customerData[0]);
            int customerId = Convert.ToInt32(customerData[1]);


            for (int i = 0; i < shoppingData.Length - 1; i++)
            {
                shoppinglist[i] = shoppingData[i].Split('-');
            }

            // Write changes customer to the database
            try
            {
                Customers customer = MovieDB.Customers.Find(customerId);

                if(customer != null)
                {
                    customer.FirstName = customerData[2];
                    customer.LastName = customerData[3];
                    customer.PhoneNo = customerData[4];
                    customer.EmailAddress = customerData[5];
                    customer.BillingAddress = customerData[6];
                    customer.BillingZip=customerData[7];
                    customer.BillingCity = customerData[8];
                    customer.DeliveryAddress = customerData[9];
                    customer.DeliveryZip = customerData[10];
                    customer.DeliveryCity = customerData[11];
                    MovieDB.Entry(customer).State = EntityState.Modified;
                    MovieDB.SaveChanges();
                }
            }
            catch
            {
                string errMsg = "Cannot find customer with customer id: " + customerId;
                throw new Exception(errMsg);
            }

            // Write changes to the OrderRows Table
            try
            {
                var orderRows = MovieDB.OrderRows.Where(or => or.OrderId == orderId).ToList();
                for (int i = 0; i < shoppinglist.Length; i++)
                {
                    int movieId = Convert.ToInt32(shoppinglist[i][0]);
                    OrderRows orderRow = MovieDB.OrderRows.FirstOrDefault(or => or.OrderId == orderId && or.MovieId == movieId);
                   
                    if (orderRow != null)
                    {
                        int quantity = Convert.ToInt32(shoppinglist[i][1]);

                        if ( quantity == 0)
                        {   //remove the OrderRow from the OrderRows Table
                            int orderRowsCount = MovieDB.OrderRows.Where(or => or.OrderId == orderId).ToList().Count();

                            MovieDB.OrderRows.Remove(orderRow);

                            if (orderRowsCount == 1)
                            {
                                //When the quantity of movie is zero and number of rows for that specfic 
                                // order is 1 then the order has to be removed too

                                Orders order = MovieDB.Orders.Find(orderId);
                                MovieDB.Orders.Remove(order);
                            }
                        }
                        else
                        {
                            // Update the OrderRows Table
                            orderRow.Price = Convert.ToDouble(shoppinglist[i][2]);
                            MovieDB.Entry(orderRow).State = EntityState.Modified;
                        }

                        MovieDB.SaveChanges();
                    }
                }

            }
            catch
            {
                string errMsg = "Cannot find the OrderRows";
                throw new Exception(errMsg);
            }
        }

        private void OrderDelete(int id)
        {
            try
            {
                var orderRow = MovieDB.OrderRows.Where(or => or.OrderId == id).ToList();
                Orders order = MovieDB.Orders.Find(id);

                if(orderRow.Count > 0)
                {
                    foreach (var or in orderRow)
                    {
                        MovieDB.OrderRows.Remove(or);
                    }
                }

                if (order != null)
                {
                    MovieDB.Orders.Remove(order);
                }

                MovieDB.SaveChanges();
            }
            catch
            {
                string errMsg = "Failed to remove the order!";
                throw new Exception(errMsg);
            }
        }

        //------------------------------------------------ Not Used ---------------------------------------------------//
        //// GET: Orders/Details/5
        //public ActionResult OrderDetails(int id)
        //{
        //    return View();
        //}

        //// GET: Orders/Create
        //public ActionResult OrderCreate()
        //{
        //    return View();
        //}

        //// POST: Orders/Create
        //[HttpPost]
        //public ActionResult OrderCreate(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}