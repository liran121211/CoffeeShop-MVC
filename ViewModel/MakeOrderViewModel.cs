using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
        private String transaction_identifier;
        private double max_price;
        private double total_order;
        private int order_time_interval; // in minutes
        private List<(Products product, double discount, int prom_id)> vip_promotions;


        private Menu selected_menu;
        private List<Menu> available_menus;
        private List<(Products, int)> selected_products;
        private List<Products> available_products;
        private List<Seats> available_seats;
        private List<(int,int)> selected_seats;
        private Users logged_in_user;
        private Timer order_timer;
        private TablesAvailabilityViewModel tables_availability;
        public MakeOrderViewModel()
        {
            this.selected_menu = null;
            this.available_menus = new List<Menu>();
            this.selected_products = new List<(Products, int)>();
            this.available_products = new List<Products>();
            this.available_seats = new List<Seats>();
            this.selected_seats = new List<(int,int)>();
            this.vip_promotions = new List<(Products product, double discount, int prom_id)>();
            this.logged_in_user = null;
            this.order_timer = null;
        }

        public DateTime CurrentDate { get => current_date; set => current_date = value; }
        public string Location { get => location; set => location = value; }
        public String Category { get => category; set => category = value; }
        public double MaxPricePerProduct { get => max_price; set => max_price = value; }
        public Menu SelectedMenu { get => selected_menu; set => selected_menu = value; }
        public List<Products> AvailableProducts { get => available_products; set => available_products = value; }
        public List<Menu> AvailableMenus { get => available_menus; set => available_menus = value; }
        public Users LoggedInUser { get => logged_in_user; set => logged_in_user = value; }
        public double TotalOrder { get => total_order; set => total_order = value; }
        public List<(Products, int)> SelectedProducts { get => selected_products; set => selected_products = value; }
        public List<Seats> AvailableSeats { get => available_seats; set => available_seats = value; }
        public List<(int,int)> SelectedSeats { get => selected_seats; set => selected_seats = value; }
        public int OrderTimeInterval { get => order_time_interval; set => order_time_interval = value; }
        public string TransactionIdentifier { get => transaction_identifier; set => transaction_identifier = value; }
        public Timer OrderTimer { get => order_timer; set => order_timer = value; }
        public TablesAvailabilityViewModel TablesAvailability { get => tables_availability; set => tables_availability = value; }
        public List<(Products product, double discount, int prom_id)> Promotions { get => vip_promotions; set => vip_promotions = value; }
    }
}