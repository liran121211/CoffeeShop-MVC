using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class TablesAvailabilityDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TablesAvailability>().ToTable("tables_availability");
        }

        public DbSet<TablesAvailability> dalTablesAvailability { get; set; }
    }
}