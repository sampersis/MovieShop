using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Helpers;

namespace MovieShop.Models
{
    public class Movies
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string RealYear { get; set; }
        public decimal Price { get; set; }
        public string Url { get; set; }
        public byte[] Image { get; set; }

        [NotMapped]
        public WebImage ImgFile;
    }
}