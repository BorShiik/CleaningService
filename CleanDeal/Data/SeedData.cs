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

            const string cleanerEmail = "Cleaner@test.com";
            const string cleanerPass = "Cleaner123$";

            const string clientMarcin = "client2@cleaning.local";
            const string marcinPass = "Marcin123$";

            const string clientAnna = "client3@cleaning.local";
            const string annaPass = "Anna123$";

           /* var clientMarcinUser = new ApplicationUser
            {
                UserName = clientMarcin,
                Email = clientMarcin,
                EmailConfirmed = true,
                FullName = "Marcin",
                Gender = Gender.Mężczyzna
            };

            var clientAnnaUser = new ApplicationUser
            {
                UserName = clientAnna,
                Email = clientAnna,
                EmailConfirmed = true,
                FullName = "Anna",
                Gender = Gender.Kobieta
            };

            await userManager.CreateAsync(clientMarcinUser, marcinPass);
            await userManager.AddToRoleAsync(clientMarcinUser, "Client");

            await userManager.CreateAsync(clientAnnaUser, annaPass);
            await userManager.AddToRoleAsync(clientAnnaUser, "Client");*/

            var cleaner = await userManager.FindByEmailAsync(cleanerEmail);
            if(cleaner is null)
            {
                cleaner = new ApplicationUser
                {
                    UserName = cleanerEmail,
                    Email = cleanerEmail,
                    EmailConfirmed = true,
                    FullName = "Cleaner Test"
                };
                var result = await userManager.CreateAsync(cleaner, cleanerPass);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(cleaner, "Cleaner");
            }

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
                    new() { Name = "Biała Kuchnia +",
                            Description = "Kompleksowe odtłuszczanie kuchni. Przywracamy blask" +
                            " i higienę wszystkich powierzchni w kuchni" + "Zakres prac: " +
                            "szafki, fronty, płytki, sprzęt AGD, blaty, podłoga, rozjaśnianie fug",
                            BasePrice = 279 },
                    new() { Name = "Mikro-Mgiełka",
                            Description = "Dezynfekcja i odświeżanie blatów. Eko-rozpylacz + rękawiczka =" +
                            " sterylna powierzchnia w 15 min. Zakres prac: " +
                            "blaty, stoły, powierzchnie robocze, klamki, uchwyty",
                            BasePrice = 59 },
                    new() { Name = "Sala Ready",
                            Description = "Serwis biura / sali konferencyjnej. Lśniący stół, zero okruszków, pachnąca przestrzeń spotkań. " +
                            "Zakres prac: przecieranie mebli, dezynfekcja krzeseł, zamiatanie/odkurzanie, kosze",
                            BasePrice = 139 },
                    new() { Name = "Plam-Stop", 
                            Description = "Punktowe usuwanie rozlewów i śladów. Natychmiastowa interwencja, aby posadzka nie ucierpiała. " +
                            "Zakres prac: usunięcie plamy, odtłuszczenie, neutralizacja zapachu, polerka",
                            BasePrice =  89 },
                    new() { Name = "Ogród w Kadrze",
                            Description = "Mycie szkieletu altan i domków. Zielony domek znowu wygląda jak nowy – bez mchu i kurzu. " +
                            "Zakres prac: mycie ciśnieniowe ścian, dachu, czyszczenie okienek, impregnacja",
                            BasePrice =  199 },
                    new() { Name = "Shaggy Fresh",
                            Description = "Pranie dywanów z długim włosem. Miękkość i zapach świeżości bez resztek detergentów. " +
                            "Zakres prac: odkurzanie, aktywna piana, ekstrakcja wodna, suszenie turbo",
                            BasePrice =  149 },
                    new() { Name = "Zen Bedroom",
                            Description = "Odkurzanie & pielęgnacja sypialni. Kurz znika, zostaje spokój – idealne warunki do snu. " +
                            "Zakres prac: ścieranie mebli, odkurzanie materaca, mycie podłogi, rośliny",
                            BasePrice =  169 },
                    new() { Name = "Lśniąca Łazienka 360",
                            Description = "Kabina, fugi i armatura bez kamienia ani zacieków. " +
                            "Zakres prac: odkamienianie, polerowanie szkła, dezynfekcja toalety, podłoga",
                            BasePrice =  189 },
                    new() { Name = "Przeprowadzka Pro",
                            Description = "Sprzątanie po wyprowadzce. Oddaj mieszkanie jak nowe, odzyskaj kaucję bez stresu. " +
                            "Zakres prac: mycie wszystkich pomieszczeń, okien, szafek, punktowe naprawki",
                            BasePrice =  349 },
                };
                await context.ServiceTypes.AddRangeAsync(servicesList);
                await context.SaveChangesAsync();
            }

            
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(
                    new Models.Product { Name = "Uniwersalny płyn", Price = 10, StockQuantity = 200, Category = ProductCategory.Cleaner },
                    new Models.Product { Name = "Ściereczka mikrofibra", Price = 4, StockQuantity = 500, Category = ProductCategory.Cleaner },
                    new Models.Product { Name = "Worki na śmieci 60 l", Price = 12, StockQuantity = 150, Category = ProductCategory.Cleaner },
                    
                    new Models.Product { Name = "Aromatyczna świeca", Price = 25, StockQuantity = 100, Category = ProductCategory.Client },
                    new Models.Product { Name = "Zestaw kadzidełek", Price = 15, StockQuantity = 80, Category = ProductCategory.Client },
                    new Models.Product { Name = "Dekoracyjny wazon", Price = 40, StockQuantity = 50, Category = ProductCategory.Client }
                );
                await context.SaveChangesAsync();
            }

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
                                                 .Where(s => s.Name == "Przeprowadzka Pro")
                                                 .Select(s => s.Id)
                                                 .FirstAsync(),
                    Address = "ul. Przykładowa 1, Kraków",
                    Cleaner = await userManager.FindByEmailAsync(cleanerEmail),
                    Date = DateTime.Today.AddDays(2).AddHours(9),
                    Status = OrderStatus.Finished,
                    IsCompleted = true,
                };

                context.CleaningOrders.Add(order);
                await context.SaveChangesAsync();

                var payment = new Payment
                {
                    CleaningOrderId = order.Id,
                    Amount = 200,
                    PaymentDate = DateTime.UtcNow,
                    ProductOrderId = null
                };
                context.Payments.Add(payment);

                var chatMessage = new ChatMessage
                {
                    CleaningOrderId = order.Id,
                    Content = "Hello, your cleaner is on the way!",
                    SenderId = admin.Id,         
                    ReceiverId = client.Id,       
                    SentAt = DateTime.UtcNow
                };

                context.ChatMessages.Add(chatMessage);

                context.Reviews.Add(new Review
                {
                    CleaningOrderId = order.Id,
                    Rating = 5,
                    Comment = "Ekipa CleanDeal zostawiła mieszkanie w lepszym stanie" +
                    " niż je odebrałam od dewelopera. Zero smug na oknach, fugi odświeżone," +
                    " nawet trudno dostępne zakamarki kuchni lśniły. Dzięki temu bez problemu odzyskałam pełną kaucję",
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();

                var clientMarcinUser = new ApplicationUser
                {
                    UserName = clientMarcin,
                    Email = clientMarcin,
                    EmailConfirmed = true,
                    FullName = "Marcin",
                    Gender = Gender.Mężczyzna
                };
                await userManager.CreateAsync(clientMarcinUser, marcinPass);
                await userManager.AddToRoleAsync(clientMarcinUser, "Client");

                var marcinOrder = new CleaningOrder
                {
                    UserId = clientMarcinUser.Id,
                    ServiceTypeId = await context.ServiceTypes
                                                   .Where(s => s.Name == "Biała Kuchnia +")
                                                   .Select(s => s.Id)
                                                   .FirstAsync(),
                    Address = "ul. Wiosenna 2, Kraków",
                    Cleaner = await userManager.FindByEmailAsync(cleanerEmail),
                    Date = DateTime.Today.AddDays(3).AddHours(10),
                    Status = OrderStatus.Finished,
                    IsCompleted = true
                };
                context.CleaningOrders.Add(marcinOrder);
                await context.SaveChangesAsync();

                var paymentMarcin = new Payment
                {
                    CleaningOrderId = marcinOrder.Id,
                    Amount = 150,
                    PaymentDate = DateTime.UtcNow,
                    ProductOrderId = null
                };
                context.Payments.Add(paymentMarcin);

                var chatMessageMarcin = new ChatMessage
                {
                    CleaningOrderId = marcinOrder.Id,
                    Content = "Hello, your cleaner is on the way!",
                    SenderId = admin.Id,
                    ReceiverId = clientMarcinUser.Id,
                    SentAt = DateTime.UtcNow
                };
                context.ChatMessages.Add(chatMessageMarcin);

                context.Reviews.Add(new Review
                {
                    CleaningOrderId = marcinOrder.Id,
                    Rating = 5,
                    Comment = "Fronty i blaty wyglądają jak nowe! Zniknęły żółte naloty przy okapie," +
                    " a fugi znowu są białe. Pachnąco i bez agresywnej chemii" +
                    " – polecam każdemu miłośnikowi jasnych kuchni",
                    CreatedAt = DateTime.UtcNow
                });

                var clientAnnaUser = new ApplicationUser
                {
                    UserName = clientAnna,
                    Email = clientAnna,
                    EmailConfirmed = true,
                    FullName = "Anna",
                    Gender = Gender.Kobieta
                };
                await userManager.CreateAsync(clientAnnaUser, annaPass);
                await userManager.AddToRoleAsync(clientAnnaUser, "Client");

                var annaOrder = new CleaningOrder
                {
                    UserId = clientAnnaUser.Id,
                    ServiceTypeId = await context.ServiceTypes
                                                   .Where(s => s.Name == "Zen Bedroom")
                                                   .Select(s => s.Id)
                                                   .FirstAsync(),
                    Address = "ul. Letnia 5, Warszawa",
                    Cleaner = await userManager.FindByEmailAsync(cleanerEmail),
                    Date = DateTime.Today.AddDays(4).AddHours(14),
                    Status = OrderStatus.Finished,
                    IsCompleted = true
                };
                context.CleaningOrders.Add(annaOrder);
                await context.SaveChangesAsync();

                var paymentAnna = new Payment
                {
                    CleaningOrderId = annaOrder.Id,
                    Amount = 180,
                    PaymentDate = DateTime.UtcNow,
                    ProductOrderId = null
                };
                context.Payments.Add(paymentAnna);

                var chatMessageAnna = new ChatMessage
                {
                    CleaningOrderId = annaOrder.Id,
                    Content = "Hello, your cleaner is on the way!",
                    SenderId = admin.Id,
                    ReceiverId = clientAnnaUser.Id,
                    SentAt = DateTime.UtcNow
                };
                context.ChatMessages.Add(chatMessageAnna);

                context.Reviews.Add(new Review
                {
                    CleaningOrderId = annaOrder.Id,
                    Rating = 4,
                    Comment = "Delikatne środki czystości nie podrażniły moich alergii;" +
                    " ekipa zadbała nawet o rośliny przy oknie." +
                    " Mały minus za pominięcie jednego gniazdka przy wycieraniu, ale poza tym idealnie",
                    CreatedAt = DateTime.UtcNow
                });


                await context.SaveChangesAsync();

            }

        }
    }
}
