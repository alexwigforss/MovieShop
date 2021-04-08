using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class OrderRowModel
    {
        public int Id { get; set; }

        //Foreign
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }

        //Foreign
        public int MovieId { get; set; }
        public MovieModel Movie { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}