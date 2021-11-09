using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetAssignment3_MovieProject.Models
{
    public class Customers
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BillingAddress { get; set; }
        public string BillingZip { get; set; }
        public string BillingCity { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryZip { get; set; }
        public string DeliveryCity { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNo { get; set; }
    }
}