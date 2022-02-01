using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class MenuProductsDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MenuProducts>().ToTable("menu_products");
        }

        public DbSet<MenuProducts> dalMenuProducts { get; set; }
    }
}