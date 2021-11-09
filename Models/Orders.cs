using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetAssignment3_MovieProject.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
    }
}