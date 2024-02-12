using System;
using System.Data.Common;

namespace Simple.Data.Abstractions;

/// <summary>
/// 
/// </summary>
public abstract class DataService : IUnitOfWork
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    protected IUnitOfWork UnitOfWork = null!;

    #endregion Properties


    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    protected DataService()
    {

    }

    #endregion Constructors


    #region Public Methods

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    public DbTransaction BeginTransaction() => UnitOfWork.BeginTransaction();


    /// <summary>
    /// Commits the database transaction.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    public void CommitTransaction() => UnitOfWork.CommitTransaction();


    /// <summary>
    /// Rolls back a transaction from a pending state.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    public void RollbackTransaction() => UnitOfWork.RollbackTransaction();

    #endregion Public Methods
}
