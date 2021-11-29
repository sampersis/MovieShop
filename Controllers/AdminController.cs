using System;
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

namespace MovieShop.Controllers
{
    public class AdminController : Controller
    {
        public ApplicationDbContext MovieDB;
        public List<Movies> Movies;
        public List<Customers> Customers;
        public List<Orders> Orders;
        public List<OrderRows> OrderRows;

        public AdminController()
        {
            MovieDB = new ApplicationDbContext();
            Movies = new List<Movies>();
            Customers = new List<Customers>();
            Orders = new List<Orders>();
            OrderRows = new List<OrderRows>();
        }

        // GET: Admin

        public ActionResult AdminLoginPage()
        {
            // If login Failed inform the user
            // if login succeeded write a welcome page and add a logout menu item to the navigation bar.
            return View();
        }
        
        //Movie Admin
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
                    ImgFile =  WebImage.GetImageFromRequest(),
                }
             ) ;

            Movie.Image = Movie.ImgFile.GetBytes();
            MovieDB.SaveChanges();

            return RedirectToAction("MovieAdminPage");
        }

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
        public ActionResult CustomerAdminPage(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Customers = MovieDB.Customers.OrderBy(c => c.LastName).ToList();

            decimal numberofRows = Customers.Count;
            decimal pageSize = 5;
            int numberOfPages = (int) Math.Ceiling(numberofRows / pageSize);

            //int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(Customers.ToPagedList(pageNumber, (int) pageSize));
        }

        // ------------------------------- Microsoft generated methods ------------------------//
        // GET: Customers/Create
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
        public ActionResult OrderAdminPage() 
        {
            Orders = MovieDB.Orders.OrderBy(c => c.Id).ToList();
            return View(Orders);

        }
    }
}