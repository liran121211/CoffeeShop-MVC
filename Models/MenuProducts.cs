using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace CoffeeShop.Models
{
    public class MenuProducts
    {
        [Key, Column(Order = 0)]
        public int Menu_Id { get; set; }

        [Key, Column(Order = 1)]
        public int Product_Id { get; set; }

    }
}