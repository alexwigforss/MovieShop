using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        // Foreign Key
        public Customer Customer { get; set; }
        [Required,DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}