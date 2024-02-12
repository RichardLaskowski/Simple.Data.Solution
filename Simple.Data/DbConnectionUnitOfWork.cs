using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using CommunityToolkit.Diagnostics;

using Simple.Data.Abstractions;

namespace Simple.Data;
public class DbConnectionUnitOfWork : IUnitOfWork
{
    #region Fields

    private readonly DbConnection _dbConnection;
    private DbTransaction? _dbTransaction;

    #endregion Fields


    #region Constructors

    public DbConnectionUnitOfWork(DbConnection dbConnection)
    {
        Guard.IsNotNull(dbConnection);

        this._dbConnection = dbConnection;
    }

    #endregion Constructors


    #region Public Methods

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    public DbTransaction BeginTransaction()
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }

        _dbTransaction = _dbConnection.BeginTransaction();

        return _dbTransaction;
    }

    public async Task<DbTransaction> BeginTransactionAsync()
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }

        _dbTransaction = await _dbConnection.BeginTransactionAsync();

        return _dbTransaction;
    }

    /// <summary>
    /// Commits the database transaction.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    public void CommitTransaction()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>("A transaction has not been initialized.");
        }

        _dbTransaction.Commit();
    }

    public async Task CommitTransactionAsync()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>("A transaction has not been initialized.");
        }

        await _dbTransaction.CommitAsync();
    }

    /// <summary>
    /// Rolls back a transaction from a pending state.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    public void RollbackTransaction()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>("A transaction has not been initialized.");
        }

        _dbTransaction.Rollback();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_dbTransaction is null)
        {
            ThrowHelper.ThrowInvalidOperationException<IDbTransaction>("A transaction has not been initialized.");
        }

        await _dbTransaction.RollbackAsync();
    }

    #endregion Public Methods
}
