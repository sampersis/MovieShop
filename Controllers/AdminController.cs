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

namespace MovieShop.Controllers
{
    public class AdminController : Controller
    {
        public ApplicationDbContext MovieDB;
        public List<Movies> Movies;

        public AdminController()
        {
            MovieDB = new ApplicationDbContext();
            Movies = new List<Movies>();
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

        public  void UploadImage(string filename)
        {
                Image img = new Image();
                img.ImageTitle = fileName;

                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                img.ImageData = ms.ToArray();

                ms.Close();
                ms.Dispose();

                //db.Images.Add(img);
                //db.SaveChanges();
        }

        [HttpPost]
        public ActionResult CreateMovie(Movies movie)
        {
           MovieDB.Movies.Add(new Movies
                {
                    Title = movie.Title,
                    Director = movie.Director,
                    RealYear = movie.RealYear,
                    Price = movie.Price,
                    Url = movie.Url,
                    Image = movie.Image
                }
            );

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
            Movie = MovieDB.Movies.FirstOrDefault(m => m.Id == movie.Id);

            if (Movie == null)
            {
                return HttpNotFound();
            }
            else
            {
                MovieDB.Movies.AddOrUpdate(new Movies
                {
                    Title = movie.Title,
                    Director = movie.Director,
                    RealYear = movie.RealYear,
                    Price = movie.Price,
                    Url = movie.Url,
                    Image = movie.Image
                }
                );

                MovieDB.Movies.Remove(Movie);
                MovieDB.SaveChanges();
                return RedirectToAction("MovieAdminPage");
            }
        }

        public ActionResult CustomerAdminPage()
        {
            return View();

        }

        //Order Admin
        public ActionResult OrderAdminPage() 
        {
            return View();

        }
    }
}