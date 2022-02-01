using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Availability")]
        public Boolean Availability { get; set; }

        [Required]
        [Display(Name = "Rank")]
        public int Rank { get; set; }

        [Required]
        [Display(Name = "AgeLimited")]
        public Boolean AgeLimited { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Image")]
        public string Image { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }
    }

    public enum ProductBoolean
    {
        True,
        False
    }

    public enum ProductCategory
    {
        Drinks,
        Meals,
        Special,
        Alcohol
    }
}




