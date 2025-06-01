using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using CleanDeal.Data;
using CleanDeal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe.Climate;

namespace CleanDeal.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Client", "Cleaner" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

           
            const string adminEmail = "admin@cleaning.local";
            const string adminPass = "Admin123$";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrator"
                };
                var result = await userManager.CreateAsync(admin, adminPass);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

           
            if (!await context.ServiceTypes.AnyAsync())
            {
                var servicesList = new List<ServiceType>
                {
                    new() { Name = "Sprzątanie generalne",   BasePrice = 200 },
                    new() { Name = "Sprzątanie lokalne",     BasePrice = 150 },
                    new() { Name = "Mycie okien",            BasePrice = 120 },
                    new() { Name = "Czyszczenie piekarnika", BasePrice =  80 },
                    new() { Name = "Czyszczenie mikrofali",  BasePrice =  60 }
                };
                await context.ServiceTypes.AddRangeAsync(servicesList);
                await context.SaveChangesAsync();
            }

          /*  //-------------------------------------------------
            // 4.  Produkty demo
            //-------------------------------------------------
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(
                    new Product { Name = "Uniwersalny płyn", Price = 10, StockQuantity = 200 },
                    new Product { Name = "Ściereczka mikrofibra", Price = 4, StockQuantity = 500 },
                    new Product { Name = "Worki na śmieci 60 l", Price = 12, StockQuantity = 150 }
                );
                await context.SaveChangesAsync();
            }*/

            if (!await context.CleaningOrders.AnyAsync())
            {
                var client = new ApplicationUser
                {
                    UserName = "client@cleaning.local",
                    Email = "client@cleaning.local",
                    EmailConfirmed = true,
                    FullName = "Jan Klient"
                };
                await userManager.CreateAsync(client, "Client123$");
                await userManager.AddToRoleAsync(client, "Client");

                var order = new CleaningOrder
                {
                    UserId = client.Id,
                    ServiceTypeId = await context.ServiceTypes
                                                 .Where(s => s.Name == "Sprzątanie generalne")
                                                 .Select(s => s.Id)
                                                 .FirstAsync(),
                    Address = "ul. Przykładowa 1, Kraków",
                    Date = DateTime.Today.AddDays(2).AddHours(9),
                    Status = OrderStatus.Finished,
                    IsCompleted = true
                };
                context.CleaningOrders.Add(order);
                await context.SaveChangesAsync();

                var payment = new Payment
                {
                    CleaningOrderId = order.Id,
                    Amount = 200,
                    PaymentDate = DateTime.UtcNow
                };
                context.Payments.Add(payment);

                var chatMessage = new ChatMessage
                {
                    CleaningOrderId = order.Id,
                    Content = "Hello, your cleaner is on the way!",
                    SenderId = client.Id,         
                    ReceiverId = admin.Id,       
                    SentAt = DateTime.UtcNow
                };

                context.ChatMessages.Add(chatMessage);

                await context.SaveChangesAsync();
            }
        }
    }
}
