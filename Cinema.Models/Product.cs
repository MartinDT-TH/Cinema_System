using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        //public string NameProduct { get; set; }
        public string? Description { get; set; }
        [Required]
        [EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; }
        //public string ProductType { get; set; }
        [Required]
        
        [Range(0.00, 9999999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; } 
        //public double Price { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; } = 0;

        public int CountProduct { get; set; } = 0;

        [ValidateNever]
        public string? ProductImage { get; set; }
    }
    public enum ProductType
    {
        Snack,
        Drink,
        Gift
    }
}