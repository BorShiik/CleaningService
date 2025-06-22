using CleanDeal.Data;
using CleanDeal.Hubs;
using CleanDeal.Repositories;
using CleanDeal.Models;
using CleanDeal.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanDeal.Services.Email;
using CleanDeal.Models.Email;
using Microsoft.Extensions.Options;
using Stripe;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using CleanDeal.Services.Loyalty;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                                   optional: true)
                     .AddEnvironmentVariables();

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

StripeConfiguration.ApiKey =
    builder.Configuration["Stripe:SecretKey"];

builder.Services.AddDbContext<CleanDeal.Data.ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var urls = builder.Configuration["ASPNETCORE_URLS"]?.Split(';').FirstOrDefault() ?? "http://localhost:5000";
var port = new Uri(urls).Port;

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = $".CleanDeal.Auth{port}";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.Name = $".CleanDeal.Session{port}";
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICleaningOrderRepository, CleaningOrderRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
builder.Services.AddScoped<IProductOrderRepository, ProductOrderRepository>();
builder.Services.AddScoped<ILoyaltyRepository, LoyaltyRepository>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton<TemplateRenderer>();
builder.Services.AddTransient<IEmailSender>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<EmailSettings>>().Value;

    return (IEmailSender)(opts.Provider?.ToLowerInvariant() switch
    {
        "Resend" => ActivatorUtilities.CreateInstance<ResendEmailSender>(sp)
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();                     
    await SeedData.InitializeAsync(scope.ServiceProvider); 
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
   /* app.UseSwagger();
    app.UseSwaggerUI();*/
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var pl = new CultureInfo("pl-PL");

var options = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(pl),
    SupportedCultures = new[] { pl },
    SupportedUICultures = new[] { pl }
};

app.UseRequestLocalization(options);

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
