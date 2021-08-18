using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Removeable

namespace MovieShop.Models.ViewModels
{
    public class FrontPageVM
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int TimesSold { get; set; }
        // public decimal Price { get; set; }
    }
}