using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class SerializedOrdersDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SerializedOrders>().ToTable("serialized_orders");
        }
        public DbSet<SerializedOrders> dalSerializedOrders { get; set; }
    }
}