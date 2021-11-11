using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class OrderRows
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MovieId { get; set; }
        public double Price { get; set; }

    }
}