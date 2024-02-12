using CommunityToolkit.Diagnostics;

using Simple.Data.Abstractions;
using Simple.Data.EntityFrameworkCore.Abstractions;

namespace Simple.Data.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
public class DbContextService : DataService
{
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    protected readonly AuditableDbContext AuditableDbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditableDbContext"></param>
    public DbContextService(AuditableDbContext auditableDbContext) : base()
    {
        Guard.IsNotNull<AuditableDbContext>(auditableDbContext);

        AuditableDbContext = auditableDbContext;
        this.UnitOfWork = new DbContextUnitOfWork(auditableDbContext);
    }

    #endregion Constructors
}
