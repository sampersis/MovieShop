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
using System.Net.Mail;

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

        // The most popular movies based on the number of orders made
        private string[][] TheTopFiveMostPopularMovies()
        {
            int numOfMoviePurchased = 0; int i = 0;
            Dictionary<int,int> PurchasePerMovie = new Dictionary<int,int>();
            Dictionary<int, int> TopFiveMovies = new Dictionary<int, int>();
            string [][] topMovies = new string [5][];
            Movies movie = new Movies();

            // Find the orders and create a sorted Dictionaries with MovieIds and number of purchases
            List<IGrouping<int,OrderRows>> orderRowsGroups = MovieDB.OrderRows.OrderBy(or => or.MovieId).GroupBy(or => or.MovieId).ToList();

            foreach (var OrderRowGroup in orderRowsGroups)
            {
                numOfMoviePurchased = 0;

                foreach (var order in OrderRowGroup)
                {
                    movie = (Movies) MovieDB.Movies.Find(order.MovieId);
                    numOfMoviePurchased += Convert.ToInt32(order.Price / (double) movie.Price);
                    PurchasePerMovie[movie.Id] = numOfMoviePurchased;
                }
            }

            var Top5Movies = PurchasePerMovie.OrderByDescending(kvp => kvp.Value).Take(5);
            TopFiveMovies = Top5Movies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Then find the movies on the top
            foreach (KeyValuePair<int,int> TopMovie in TopFiveMovies)
            {
                movie = (Movies)MovieDB.Movies.Find(TopMovie.Key);
                topMovies[i]= new string [] { movie.Title, TopMovie.Value.ToString() };
                i++;
            }

            //return the list of the most popular movies
            return topMovies;
        }

        // The 5 newest movies (based on release year)
        private List<Movies> TheTopFiveNewestMovies()
        {
            //Sort all movies according to release year
            List <Movies> TheTopFiveNewestMovies = MovieDB.Movies.OrderByDescending(x => x.RealYear).Take(5).ToList();

            // Return the list
            return TheTopFiveNewestMovies;
        }

        // The 5 oldest movies (based to release year)
        private List<Movies> TheFiveOldestMovies()
        {
            //Sort all movies according to release year
            List<Movies> TheFiveOldestMovies = MovieDB.Movies.OrderBy(m => m.RealYear).Take(5).ToList();

            // Return the list
            return TheFiveOldestMovies;
        }

        // The 5 cheapest movies (based to price)
        private List<Movies> TheFiveCheapestMovies()
        {
            //Sort all movies according to price
            List<Movies> TheFiveCheapestMovies = MovieDB.Movies.OrderBy(m => m.Price).Take(5).ToList();

            // Return the list
            return TheFiveCheapestMovies;
        }

        // The customer who have made the most expensive order (based of the sum of the order rows in an order)
        private string TheCustomerWhoHaveMadeTheMostExpensiveOrder()
        {
            Customers TheTopCustomer = new Customers();
            Dictionary<int, double> TheWinner = new Dictionary<int, double>();
            Dictionary<int, double> CustomerPurchaseSummary = new Dictionary<int, double>();
            var CustomerOrders = MovieDB.Orders.OrderByDescending(o => o.Id).GroupBy(o => o.CustomerId);
            var orderRows = MovieDB.OrderRows.GroupBy(or => or.OrderId).Select(g => new {oid = g.Key , sum = g.Sum(or=>or.Price)}).ToList();

            foreach (var customerOrder in CustomerOrders)
            {
                double sum = 0;
                foreach(var order in customerOrder)
                {
                    foreach (var orderRow in orderRows)
                    {
                        if (orderRow.oid == order.Id)
                        {
                            sum += orderRow.sum;
                            CustomerPurchaseSummary[order.CustomerId] = sum;
                        }
                    }
                }
            }

            TheWinner = CustomerPurchaseSummary.OrderByDescending(kvp => kvp.Value).Take(1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            TheTopCustomer = (Customers)MovieDB.Customers.Find(TheWinner.Keys.First());

            return TheTopCustomer.FirstName + " " + TheTopCustomer.LastName + " with a SEK " + TheWinner.Values.First().ToString() + " Purchase";

        }

        //Load the Movie Shop
        public ActionResult MovieShop()
        {
            Movies = MovieDB.Movies.ToList();

            Session["ID"] = Guid.NewGuid().ToString();
            if (Session["Loading"] == null)
            {
                Session["Loading"] = null;
            }

            if (Movies == null)
            {
                return View();
            }
            else
            {
                ViewBag.TheTopFiveMostPopularMovies = TheTopFiveMostPopularMovies();
                ViewBag.TheTopFiveNewestMovies = TheTopFiveNewestMovies();
                ViewBag.TheFiveOldestMovies = TheFiveOldestMovies();
                ViewBag.TheFiveCheapestMovies = TheFiveCheapestMovies();
                ViewBag.TheCustomerWhoHaveMadeTheMostExpensiveOrder = TheCustomerWhoHaveMadeTheMostExpensiveOrder();

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

        [HttpPost, ActionName("Contact")]
        public ActionResult SendEmail()
        {
            string sender = @"movie.shop.inc@gmail.com";
            string password = @"C95*c%6?!q";
            string contactee = Request.Form["contact-name"];
            string recepient = @"movie.shop.inc@gmail.com";
            string contacteeEmail = Request.Form["contact-email"];
            string msg = Request.Form["contact-message"];
            string sendCopy = Request.Form["send-a-copy-of-email-to-me-too"];
            string subject = "Message from " + contactee + " (" + DateTime.Now.ToString("yyyy'-'MM'-'dd") + ")" ;
            string message = "Message from " + contactee + " " + contacteeEmail + " .\n\n\n" + " " + msg;

            try
            {
                MailMessage Message = new MailMessage()
                {
                    From = new MailAddress(sender),
                    Subject = subject,
                    IsBodyHtml = false,
                    Body = message
                };

                Message.To.Add(new MailAddress(recepient));

                if (sendCopy == "Checked")
                {
                    Message.To.Add(new MailAddress(contacteeEmail));
                }

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                };

                smtpClient.Send(Message);
            }
            catch (Exception e)
            {
                e.Data["DateTimeInfo"] = DateTime.Now.ToString("yyyy'-'MM'-'dd");
                Console.WriteLine(e.Data["DateTimeInfo"] + ": " + e.Message);
                Environment.Exit(-1);
            }

            return View("Contact");
        }

        
        // Generates a random password for the customer
        public string CreatePassword(int length)
        {
                const string validCharactersInPassword = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#¤%&/()=\}][{<>|-_@";
                StringBuilder password = new StringBuilder();
                Random rnd = new Random();
                while (0 < length--)
                {
                    password.Append(validCharactersInPassword[rnd.Next(validCharactersInPassword.Length)]);
                }
                return password.ToString();
        }

        //Sends an email with account login information to the customer
        public void SendEmailWithUsernameAndPassword(string customerEmail,string customerName)
        {
            string sender = @"movie.shop.inc@gmail.com";
            string password = @"C95*c%6?!q";
            string contactee = customerName;
            string recepient = customerEmail;
            string subject = "Welcome to Movie Shop Inc";
            string message = "Dear " + customerName + "," + Environment.NewLine +
                "Thank you for your purchase. As per your request, we have created a Movie Shop Inc. account for you, " + 
                "details of which you can find below: " + Environment.NewLine + Environment.NewLine +
                "username: " + recepient + Environment.NewLine +
                "password: " + CreatePassword(12) + Environment.NewLine + Environment.NewLine +
                "please note that your password is autogenerated and is valid for a duration of 48 hours only." + Environment.NewLine +
                "We appreciate it if you also confirm your email here. Your account will be activitaed once you have confirmed your email" +
                "and logged in your account within 48 hours of receiving this emial, please." + Environment.NewLine + Environment.NewLine +
                "If you have any further questions, please do not hesitate to contact our service desk by either email, servicedesk@movieshopinc.com" +
                ", or by phone 08345345666" + Environment.NewLine + Environment.NewLine +
                "Best regards," + Environment.NewLine + "Movie Shop Inc. Team" + Environment.NewLine;

            try
            {
                MailMessage Message = new MailMessage()
                {
                    From = new MailAddress(sender),
                    Subject = subject,
                    IsBodyHtml = false,
                    Body = message
                };

                Message.To.Add(new MailAddress(recepient));

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sender, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                };

                smtpClient.Send(Message);
            }
            catch (Exception e)
            {
                e.Data["DateTimeInfo"] = DateTime.Now.ToString("yyyy'-'MM'-'dd");
                Console.WriteLine(e.Data["DateTimeInfo"] + ": " + e.Message);
                Environment.Exit(-1);
            }
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
            Orders order = new Models.Orders();
            OrderRows orderRow = new OrderRows();
            Boolean CreateAccount = Convert.ToBoolean(Request.Form["CreateAccountCheckBox"]);
            String[][] shoppingList = ShoppingList(Request.Form["FinalShoppingList"]);
            Session["ShoppingCart"] = ShoppingList(Request.Form["FinalShoppingList"]);

            //// Insert Customer data in the database, if the customer does not exist
            //Check whether User has already an account in AspNetUsers
            if (CreateAccount)
            {
                SendEmailWithUsernameAndPassword(customer.EmailAddress, customer.FirstName + " " + customer.LastName);
            }

            // Check whether user is already in Customers Table
            Customers Customer = MovieDB.Customers.FirstOrDefault(c => c.EmailAddress == customer.EmailAddress);

            if (Customer == null)
            {
                MovieDB.Customers.Add(customer);
                MovieDB.SaveChanges();
                Customer = MovieDB.Customers.FirstOrDefault(c => c.EmailAddress == customer.EmailAddress);
            }


                order.CustomerId = Customer.Id;
                order.OrderDate = DateTime.Now;
                MovieDB.Orders.Add(order);
                MovieDB.SaveChanges();

                // Populate the OrderRow Table
                //order = MovieDB.Orders.FirstOrDefault(o => o.CustomerId == Customer.Id);

                for (int i=0; i < shoppingList.Length; i++)
                {
                    string movieTitle = shoppingList[i][0];
                    string moviePrice = shoppingList[i][3];
                    Movies movie = (Movies)MovieDB.Movies.FirstOrDefault(m => m.Title == movieTitle);

                    if (movie != null)
                    {
                        orderRow.OrderId = order.Id;
                        orderRow.MovieId = movie.Id;
                        orderRow.Price = Convert.ToDouble(moviePrice);

                        MovieDB.OrderRows.Add(orderRow);
                        MovieDB.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Unable to find the movie!");
                    }
                }


            // Load a page with the Order Details
            string prefixZeroStr = "";
            int lengthOfOrderId = order.Id.ToString().Length;
            int numberofZeros = 10 - lengthOfOrderId;
            for (int i = 0; i < numberofZeros; i++) prefixZeroStr += "0";
            ViewBag.OrderId = prefixZeroStr+order.Id;
            ViewBag.Date = order.OrderDate;
            ViewBag.Customer = Customer;
            ViewBag.Payment = "Master Card";
            //ViewBag.FinalShoppingList = Session["ShoppingCart"];

            return View();
        }
    }
}
