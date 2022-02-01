using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class TablesAvailability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sunday")]
        public Boolean Sunday { get; set; }

        [Required]
        [Display(Name = "Monday")]
        public Boolean Monday { get; set; }

        [Required]
        [Display(Name = "Tuesday")]
        public Boolean Tuesday { get; set; }

        [Required]
        [Display(Name = "Wednesday")]
        public Boolean Wednesday { get; set; }

        [Required]
        [Display(Name = "Thursday")]
        public Boolean Thursday { get; set; }

        [Required]
        [Display(Name = "Friday")]
        public Boolean Friday { get; set; }

        [Required]
        [Display(Name = "Saturday")]
        public Boolean Saturday { get; set; }


    }
}