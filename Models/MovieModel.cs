using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieShop.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        [Required,StringLength(200)]
        public string Title { get; set; }
        [Required, StringLength(100)]
        public string Director { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        [Required, StringLength(100)]
        public string LeadActor { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required, DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
    }
}