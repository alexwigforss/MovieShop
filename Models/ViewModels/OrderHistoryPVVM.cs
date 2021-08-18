using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieShop.Models.ViewModels
{
    public class OrderHistoryPVVM
    {
        public int OrderId { get; set; }
        // public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderHistoryListVM> OrderHistoryList { get; set; }
        public OrderHistoryPVVM()
        {
            OrderHistoryList = new List<OrderHistoryListVM>();
        }
    }
}