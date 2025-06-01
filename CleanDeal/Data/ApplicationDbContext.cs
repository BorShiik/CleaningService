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

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            b.Entity<ServiceType>().Property(s => s.BasePrice).HasPrecision(18, 2);

            b.Entity<CleaningOrder>()
                .HasOne(o => o.User)
                .WithMany(u => u.ClientOrders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<CleaningOrder>()
                .HasOne(o => o.AssignedCleaner)
                .WithMany(u => u.CleanerOrders)
                .HasForeignKey(o => o.AssignedCleanerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ChatMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Review>()
               .HasOne(r => r.CleaningOrder)
               .WithOne(o => o.Review)
               .HasForeignKey<Review>(r => r.CleaningOrderId)
               .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
