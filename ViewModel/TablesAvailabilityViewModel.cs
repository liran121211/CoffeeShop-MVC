using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class TablesAvailabilityViewModel
    {
        public TablesAvailability vmTableAvailability { get; set; }
        public List<TablesAvailability> vmTablesAvailability { get; set; }
    }
}