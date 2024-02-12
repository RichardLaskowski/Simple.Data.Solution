using System.Data.Common;

namespace Simple.Data.Abstractions;

public interface IUnitOfWork
{
    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    DbTransaction BeginTransaction();


    /// <summary>
    /// Commits the database transaction.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    void CommitTransaction();


    /// <summary>
    /// Rolls back a transaction from a pending state.
    /// </summary>
    /// <exception cref="Exception">An error occurred while trying to commit the transaction.</exception>
    /// <exception cref="InvalidOperationException">The transaction has already been committed or rolled back. -or- The connection is broken.</exception>
    void RollbackTransaction();
}
