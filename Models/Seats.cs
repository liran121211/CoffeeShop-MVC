using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Seats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public bool Col1 { get; set; }

        [Required]
        public bool Col2 { get; set; }

        [Required]
        public bool Col3 { get; set; }

        [Required]
        public bool Col4 { get; set; }

        [Required]
        public bool Col5 { get; set; }

    }
}