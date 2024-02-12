using System.Data;
using System.Data.Common;

using CommunityToolkit.Diagnostics;

using Microsoft.EntityFrameworkCore.Storage;

using Simple.Data.Abstractions;
using Simple.Data.EntityFrameworkCore.Abstractions;

namespace Simple.Data.EntityFrameworkCore;

/// <summary>
/// An <see cref="EntityFrameworkCore"/> implementation of the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class DbContextUnitOfWork : IUnitOfWork
{
    #region Fields

    private readonly AuditableDbContext _auditableDbContext;
    private DbTransaction? _dbTransaction;

    #endregion Fields


    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditableDbContext"></param>
    public DbContextUnitOfWork(AuditableDbContext auditableDbContext)
    {
        Guard.IsNotNull<AuditableDbContext>(auditableDbContext);

        this._auditableDbContext = auditableDbContext;
    }

    #endregion Constructors


    #region Public Methods

    /// <inheritdoc/>
    public DbTransaction BeginTransaction()
    {
        IDbContextTransaction dbContextTransaction = _auditableDbContext.Database.BeginTransaction();

        _dbTransaction = dbContextTransaction.GetDbTransaction();

        return _dbTransaction;
    }

    /// <inheritdoc/>
    public void CommitTransaction()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>();
        }

        _dbTransaction.Commit();
    }

    /// <inheritdoc/>
    public void RollbackTransaction()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>();
        }

        _dbTransaction.Rollback();
    }

    DbTransaction IUnitOfWork.BeginTransaction() => throw new System.NotImplementedException();

    #endregion Public Methods
}
