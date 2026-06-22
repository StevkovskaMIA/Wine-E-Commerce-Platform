using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WineShop.Domain.DomainModels;
using WineShop.Domain.Identity;

namespace WineShop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<EShopApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductInOrder> ProductInOrders { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }
        public virtual DbSet<TastingPackage> TastingPackages { get; set; }
        public virtual DbSet<TastingReservation> TastingReservations { get; set; }
        public virtual DbSet<ProductAward> ProductAwards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TastingPackage>().HasData(
                new TastingPackage
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Класична дегустација",
                    Description = "Прошетка низ винарија, дегустација на селектирани вина и храна на даска.",
                    DurationHours = 3,
                    MaxGuests = 8,
                    Price = 1500,
                    BlocksWholeDay = false
                },
                new TastingPackage
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Премиум искуство",
                    Description = "Ексклузивна дегустација со водена тура и богато гастрономско искуство.",
                    DurationHours = 3,
                    MaxGuests = 8,
                    Price = 2800,
                    BlocksWholeDay = true
                }
            );
        }
    }
}
