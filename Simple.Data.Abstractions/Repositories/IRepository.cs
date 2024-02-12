namespace Simple.Data.Abstractions.Repositories;

/// <summary>
/// Defines an abstraction of data access.
/// </summary>
/// <typeparam name="TItem">The items in the repository.</typeparam>
public interface IRepository<TItem>
    where TItem : class
{
    /// <summary>
    /// Adds an item to the repository.
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>The item added.</returns>
    TItem Add(TItem item);


    /// <summary>
    /// Removes an item from the repository.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    void Remove(TItem item);


    /// <summary>
    /// Updates an item from the repository.
    /// </summary>
    /// <typeparam name="TId">The primary key type.</typeparam>
    /// <param name="id">The primary key.</param>
    /// <param name="item">The updated item.</param>
    void Update<TId>(TId id, TItem item);


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId">The primary key type.</typeparam>
    /// <param name="id">The primary key.</param>
    /// <returns>The item found or null.</returns>
    TItem? Get<TId>(TId id);
}