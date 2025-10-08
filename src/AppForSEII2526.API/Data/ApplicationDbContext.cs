using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet PurchaseItem > PurchaseItems { get ; set ; }
    public DbSet<Purchase> Purchase { get; set; }               //Explicitamente, tengo que asignar las clases que se tienen que asignar en la base de datos.

    protected override void OnModelCreating (ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
