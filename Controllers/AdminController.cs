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