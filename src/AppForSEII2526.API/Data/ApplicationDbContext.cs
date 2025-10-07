using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet PurchaseItem > PurchaseItems { get ; set ; }

    protected override void OnModelCreating (ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<PurchaseItem>().HasKey(pi => new {pi.PurchaseId , pi.DevideId });
    }
}
