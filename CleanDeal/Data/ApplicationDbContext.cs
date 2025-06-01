using CleanDeal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            b.Entity<ServiceType>().Property(s => s.BasePrice).HasPrecision(18, 2);

            b.Entity<CleaningOrder>()
                 .Property(o => o.TotalPrice)
                 .HasPrecision(18, 2);

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
               .HasOne(m => m.CleaningOrder)
               .WithMany(o => o.ChatMessages)
               .HasForeignKey(m => m.CleaningOrderId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Review>()
               .HasOne(r => r.CleaningOrder)
               .WithOne(o => o.Review)
               .HasForeignKey<Review>(r => r.CleaningOrderId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<OrderItem>(oi =>
            {
                oi.ToTable("OrderItems");          //  ⬅️ jawnie wymuszamy liczbę mnogą

                oi.HasKey(x => x.Id);

                oi.HasOne(x => x.CleaningOrder)
                  .WithMany(o => o.Items)
                  .HasForeignKey(x => x.CleaningOrderId)
                  .OnDelete(DeleteBehavior.Cascade);

                oi.HasOne(x => x.ServiceType)
                  .WithMany()
                  .HasForeignKey(x => x.ServiceTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

                oi.Property(x => x.Price)
                  .HasPrecision(18, 2);
            });

            /* b.Entity<OrderItem>()
                 .HasOne(oi => oi.ServiceType)
                 .WithMany() // нет отдельной навигации OrderItems в ServiceType
                 .HasForeignKey(oi => oi.ServiceTypeId)
                 .OnDelete(DeleteBehavior.Restrict);*/
        }

    }
}
