using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class MenuProductsViewModel
    {
        public MenuProducts vmMenuProduct { get; set; }
        public List<MenuProducts> vmMenuProducts { get; set; }
        public List<List<MenuProducts>> vmNestedMenuProducts { get; set; }
    }
}