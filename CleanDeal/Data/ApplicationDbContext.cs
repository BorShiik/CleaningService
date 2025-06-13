using CleanDeal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts)
            : base(opts) { }

        public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
        public DbSet<CleaningOrder> CleaningOrders => Set<CleaningOrder>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            b.Entity<ServiceType>().Property(s => s.BasePrice).HasPrecision(18, 2);
            b.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            b.Entity<CleaningOrder>()
                .Property(o => o.Status)
                .HasConversion<string>();

            b.Entity<CleaningOrder>()
               .HasOne(o => o.Cleaner)
               .WithMany(u => u.CleanerOrders)
               .HasForeignKey(o => o.CleanerId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<CleaningOrder>()
               .HasOne(o => o.Payment)
               .WithOne(p => p.CleaningOrder)
               .HasForeignKey<Payment>(p => p.CleaningOrderId)
               .OnDelete(DeleteBehavior.Cascade);

            b.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Review>()
               .HasOne(r => r.CleaningOrder)
               .WithOne(o => o.Review)
               .HasForeignKey<Review>(r => r.CleaningOrderId)
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
