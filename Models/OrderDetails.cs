using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class OrderDetails
    {
        public OrderDetails() {
            order = new Orders();
            customer = new Customers();
            shoppingCart = new List<ShoppingCart>();
        }

        public class ShoppingCart
        {
            public int movieId { get; set; }
            public string movieTitle { get; set; }
            public int qty { get; set; }
            public double price { get; set; }
            public double sum { get; set; }
        }

        public List<ShoppingCart> shoppingCart;

        public double totalSum { get; set; } = 0;
        public double vat { get; set; } = 0;

        public Orders order { get; set; }
        public Customers customer { get; set; }
    }
}