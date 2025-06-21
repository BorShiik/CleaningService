using CleanDeal.Data;
using CleanDeal.Repositories;
using CleanDeal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanDeal.Services.Email;
using CleanDeal.Models.Email;
using Microsoft.Extensions.Options;




var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                                   optional: true)
                     .AddEnvironmentVariables();

builder.Services.AddDbContext<CleanDeal.Data.ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

//builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICleaningOrderRepository, CleaningOrderRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// SignalR Hub
//app.MapHub<ChatHub>("/chatHub");

app.Run();
