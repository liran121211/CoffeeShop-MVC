using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class ProductsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Products>().ToTable("products");
        }

        public DbSet<Products> dalProducts { get; set; }
    }
}