using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class MakeOrderViewModel
    {
        private DateTime current_date;
        private String location;
        private String category;
        private double max_price;

        private Menu selected_menu;
        private List<Products> available_products;
        private List<Menu> available_menus;
        private Seats seats;
        private Users logged_in_user;
        public MakeOrderViewModel()
        {
            this.selected_menu = null;
            this.Seats = null;
            this.available_menus = new List<Menu>();
            this.available_products = new List<Products>();
            this.logged_in_user = null;
        }

        public DateTime CurrentDate { get => current_date; set => current_date = value; }
        public string Location { get => location; set => location = value; }
        public String Category { get => category; set => category = value; }
        public double MaxPricePerProduct { get => max_price; set => max_price = value; }
        public Menu SelectedMenu { get => selected_menu; set => selected_menu = value; }
        public List<Products> AvailableProducts { get => available_products; set => available_products = value; }
        public List<Menu> AvailableMenus { get => available_menus; set => available_menus = value; }
        public Seats Seats { get => seats; set => seats = value; }
        public Users LoggedInUser { get => logged_in_user; set => logged_in_user = value; }
    }
}