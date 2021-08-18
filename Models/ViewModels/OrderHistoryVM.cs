using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models.ViewModels
{
    public class OrderHistoryVM
    {
        public int OrderId { get; set; }
        public string MovieTitle { get; set; }
        public string CustomerName { get; set; }
        public int NoofCopies { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}