using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models.ViewModels
{
    public class MovieListVM
    {
        public int MovieId { get; set; }
        public string Movie { get; set; }
        public int ReleaseYear { get; set; }
        public int NoofCopies { get; set; }
        public decimal Price { get; set; }
    }
}