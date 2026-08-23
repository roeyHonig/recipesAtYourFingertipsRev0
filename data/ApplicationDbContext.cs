using Microsoft.EntityFrameworkCore;
using recipesAtYourFingertipsRev0.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace recipesAtYourFingertipsRev0.Data;

public class ApplicationDbContext : DbContext, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<ExternalLogin> ExternalLogins { get; set; }

    public DbSet<Recipe> Recipes { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> ExternalLogin relationship
        modelBuilder.Entity<ExternalLogin>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<ExternalLogin>(externalLogin => externalLogin.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Recipe relationship
        modelBuilder.Entity<Recipe>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(recipe => recipe.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // An external provider identity must be unique.
        modelBuilder.Entity<ExternalLogin>()
            .HasIndex(externalLogin => new
            {
                externalLogin.Provider,
                externalLogin.ProviderUserId
            })
            .IsUnique();
    }
}