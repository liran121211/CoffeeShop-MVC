using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class OrdersViewModel
    {
        public Orders vmOrder { get; set; }
        public List<Orders> vmOrders { get; set; }
    }
}