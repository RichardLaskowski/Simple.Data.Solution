using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Simple.Data.Abstractions;

namespace Simple.Data.EntityFrameworkCore.Abstractions;

public abstract class AuditableDbContext : DbContext
{
    #region Constructors

    protected AuditableDbContext() : base()
    {

    }

    protected AuditableDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    #endregion Constructors


    #region Public Methods

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModified = DateTime.Now;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    #endregion Public Methods
}
