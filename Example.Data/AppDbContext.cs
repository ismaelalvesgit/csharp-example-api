using Example.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Example.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Category> Category { get; set; } = default!;
    public DbSet<Product> Product { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var insertedEntries = this.ChangeTracker.Entries()
                               .Where(x => x.State == EntityState.Added)
                               .Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            var auditableEntity = insertedEntry as EntityBase;
            //If the inserted object is an Auditable. 
            if (auditableEntity != null)
            {
                var utcNow = DateTime.UtcNow;
                auditableEntity.CreatedAt = utcNow;
                auditableEntity.UpdatedAt = utcNow;
            }
        }

        var modifiedEntries = this.ChangeTracker.Entries()
                   .Where(x => x.State == EntityState.Modified)
                   .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
        {
            //If the inserted object is an Auditable. 
            var auditableEntity = modifiedEntry as EntityBase;
            if (auditableEntity != null)
            {
                auditableEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}