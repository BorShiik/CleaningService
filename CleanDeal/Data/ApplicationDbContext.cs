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
        public DbSet<ProductOrder> ProductOrders => Set<ProductOrder>();
        public DbSet<ProductOrderItem> ProductOrderItems => Set<ProductOrderItem>();
        public DbSet<CleaningOrderService> CleaningOrderServices => Set<CleaningOrderService>();
        public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();
        public DbSet<ServicePackage> ServicePackages => Set<ServicePackage>();
        public DbSet<ServicePackageItem> ServicePackageItems => Set<ServicePackageItem>();
        public DbSet<UserServiceDiscount> UserServiceDiscounts => Set<UserServiceDiscount>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            b.Entity<Payment>().Property(p => p.Tip).HasPrecision(18, 2);
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

            b.Entity<CleaningOrder>()
               .HasOne(o => o.ServiceType)
               .WithMany()
               .HasForeignKey(o => o.ServiceTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ProductOrder>()
               .HasOne(o => o.User)
               .WithMany(u => u.ProductOrders)
               .HasForeignKey(o => o.UserId);

            b.Entity<ProductOrder>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.ProductOrder)
                .HasForeignKey<Payment>(p => p.ProductOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ProductOrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);

            b.Entity<ProductOrderItem>()
                .HasOne(i => i.ProductOrder)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.ProductOrderId);

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

            b.Entity<CleaningOrderService>()
                 .HasOne(os => os.CleaningOrder)
                 .WithMany(o => o.ServiceItems)
                 .HasForeignKey(os => os.CleaningOrderId)
                 .OnDelete(DeleteBehavior.Cascade);

            b.Entity<CleaningOrderService>()
                .HasOne(os => os.ServiceType)
                .WithMany()
                .HasForeignKey(os => os.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Review>()
               .HasOne(r => r.CleaningOrder)
               .WithOne(o => o.Review)
               .HasForeignKey<Review>(r => r.CleaningOrderId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<LoyaltyTransaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.LoyaltyTransactions)
                .HasForeignKey(t => t.UserId);

            b.Entity<ServicePackageItem>()
                .HasOne(i => i.ServicePackage)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.ServicePackageId);

            b.Entity<ServicePackageItem>()
                .HasOne(i => i.ServiceType)
                .WithMany()
                .HasForeignKey(i => i.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<UserServiceDiscount>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            b.Entity<UserServiceDiscount>()
                .HasOne(d => d.ServiceType)
                .WithMany()
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<UserServiceDiscount>()
                .Property(d => d.Percentage)
                .HasPrecision(5, 2);

            b.Entity<ServicePackage>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }

    }
}
