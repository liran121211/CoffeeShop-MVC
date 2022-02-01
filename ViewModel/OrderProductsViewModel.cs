using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class OrderProductsViewModel
    {
        public OrderProducts vmOrderProduct { get; set; }
        public List<OrderProducts> vmOrderProducts { get; set; }
        public List<List<OrderProducts>> vmNestedOrderProducts { get; set; }
    }
}