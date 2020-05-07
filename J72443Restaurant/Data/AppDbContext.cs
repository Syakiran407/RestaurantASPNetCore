using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace J72443Restaurant.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options)
        {

        }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; }

        [NotMapped]
        public DbSet<CheckoutItem> CheckoutItems { get; set; }

        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BasketItem>().HasKey(t => new { t.StockID, t.BasketID });
            builder.Entity<OrderItem>().HasKey(t => new { t.StockID, t.OrderNo });
        }
    }
}
