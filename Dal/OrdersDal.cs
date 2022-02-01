using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class OrdersDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Orders>().ToTable("orders");
        }
        public DbSet<Orders> dalOrders { get; set; }
    }
}