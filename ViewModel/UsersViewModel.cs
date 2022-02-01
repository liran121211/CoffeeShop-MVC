using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class UsersViewModel
    {
        public Users vmUser { get; set; }
        public List<Users> vmUsers { get; set; }
    }
}