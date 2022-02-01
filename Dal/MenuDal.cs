using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CoffeeShop.Models;

namespace CoffeeShop.Dal
{
    public class MenuDal : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Menu>().ToTable("menu");
        }

        public DbSet<Menu> dalMenu { get; set; }

        public System.Data.Entity.DbSet<CoffeeShop.Models.MenuProducts> MenuProducts { get; set; }
    }
}