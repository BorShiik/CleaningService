using CleanDeal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanDeal.Data;

public class ApplicationDbContext : IdentityDbContext<Uzytkownik>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<RodzajUslugi> RodzajeUslug { get; set; }
    public DbSet<ZamowienieSprzatania> ZamowieniaSprzatania { get; set; }
    public DbSet<Platnosc> Platnosci { get; set; }
    public DbSet<WiadomoscCzat> WiadomosciCzat { get; set; }
    public DbSet<Opinia> Opinie { get; set; }
    public DbSet<Dostepnosc> Dostepnosci { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
