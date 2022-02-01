using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace CoffeeShop.Models
{
    public class SerializedOrders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SerializedData { get; set; }
    }
}