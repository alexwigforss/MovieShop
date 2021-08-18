using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models.ViewModels
{
    public class PaginateVM
    {
        public IEnumerable<Movie> Movies { get; set; }
        public int MoviesPerPage { get; set; }
        public int MoviesPerRow { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public static int TotalPages(int DBCount, int CountPerPage)
        {
            if (DBCount <= CountPerPage) return 1;
            return Convert.ToInt32(Math.Ceiling((DBCount / (double)CountPerPage)));
        }
        public PaginateVM()
        {
            Movies = new List<Movie>();
        }
    }
}