using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class OrderRow
    {
        public int Id { get; set; }

        //Foreign
        public int OrderId { get; set; }
        public Order Order { get; set; }

        //Foreign
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        public int NoofCopies { get; set; }
    }
}