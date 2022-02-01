using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class ProductsViewModel
    {
        public Products vmProduct { get; set; }
        public List<Products> vmProducts { get; set; }
    }
}