using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class MenuViewModel
    {
        public Menu vmMenu { get; set; }
        public List<Menu> vmMenus { get; set; }
        public List<List<Menu>> vmNestedMenus { get; set; }
    }
}