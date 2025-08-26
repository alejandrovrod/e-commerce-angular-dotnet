using Microsoft.EntityFrameworkCore;
using ECommerce.User.Domain.Entities;
using ECommerce.User.Domain.ValueObjects;
using ECommerce.User.Infrastructure.Data.Configurations;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.User.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.User> Users { get; set; } = default!;
    // Temporarily commented out to fix migration issues
    // public DbSet<Address> Addresses { get; set; } = default!;
    // public DbSet<UserPreferences> UserPreferences { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        // Temporarily commented out to fix migration issues
        // modelBuilder.ApplyConfiguration(new AddressConfiguration());
        // modelBuilder.ApplyConfiguration(new UserPreferencesConfiguration());

        // Global query filters for soft delete
        modelBuilder.Entity<Domain.Entities.User>().HasQueryFilter(e => !e.IsDeleted);
        // Temporarily commented out to fix migration issues
        // modelBuilder.Entity<Address>().HasQueryFilter(e => !e.IsDeleted);
        // modelBuilder.Entity<UserPreferences>().HasQueryFilter(e => !e.IsDeleted);

        // Set table names
        modelBuilder.Entity<Domain.Entities.User>().ToTable("Users");
        // Temporarily commented out to fix migration issues
        // modelBuilder.Entity<Address>().ToTable("Addresses");
        // modelBuilder.Entity<UserPreferences>().ToTable("UserPreferences");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseAuditableEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                // entity.CreatedBy = GetCurrentUserId(); // Set from current user context
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                // entity.UpdatedBy = GetCurrentUserId(); // Set from current user context
            }
        }
    }
}
