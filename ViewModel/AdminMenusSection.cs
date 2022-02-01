using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class AdminMenusSectionViewModel
    {
        Menu menu;
        List<Products> all_products;
        List<Products> current_products;
        public AdminMenusSectionViewModel(Menu menu, List<Products> all_products, List<Products> current_products)
        {
            this.menu = menu;
            this.all_products = all_products;
            this.current_products = current_products;
        }
        public AdminMenusSectionViewModel()
        {
            this.menu = null;
            this.all_products = null;
            this.current_products = null;
        }
        public Menu Menu { get => menu; set => menu = value; }
        public List<Products> All_products { get => all_products; set => all_products = value; }
        public List<Products> Current_products { get => current_products; set => current_products = value; }

    }
}