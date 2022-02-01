using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.ViewModel
{
    public class SeatsViewModel
    {
        public Seats vmSeat { get; set; }
        public List<Seats> vmSeats { get; set; }
        public List<List<Seats>> vmNestedSeats { get; set; }
    }
}