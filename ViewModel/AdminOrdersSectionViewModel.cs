using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class AdminOrdersSectionViewModel
    {
        Orders o;
        List<SelectListItem> sli;
       public AdminOrdersSectionViewModel(Orders o, List<SelectListItem> sli) {
            this.O = o;
            this.Sli = sli;
        }
        public AdminOrdersSectionViewModel()
        {
            this.O = null;
            this.Sli = null;
        }

        public Orders O { get => o; set => o = value; }
        public List<SelectListItem> Sli { get => sli; set => sli = value; }
    }
}