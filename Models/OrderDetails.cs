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
            orderRows = new List<OrderRows>();
            movie = new List<Movies>();
        }

        public Orders order { get; set; }
        public List<OrderRows> orderRows { get; set; }
       public Customers customer { get; set; }
        public List<Movies> movie { get; set; }
    }
}