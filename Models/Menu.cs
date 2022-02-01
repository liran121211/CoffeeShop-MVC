using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CoffeeShop.Models
{
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Menu Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Time")]
        public DateTime Time { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }


        [Required]
        [Display(Name = "Currently Listed")]
        public Boolean Listed { get; set; }
    }

    public enum Location
    {
        Balcony,
        Inside
    }

    public enum MenuCategory
    {
        BreakFast,
        Business,
        Alcohol,
        Dinner
    }
}