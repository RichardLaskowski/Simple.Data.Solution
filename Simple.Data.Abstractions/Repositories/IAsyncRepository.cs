using System.Threading;
using System.Threading.Tasks;

namespace Simple.Data.Abstractions.Repositories;

/// <summary>
/// Defines an extension of <see cref="IRepository{TItem}"/> that adds <see langword="async"/> methods.
/// </summary>
/// <typeparam name="TItem">The items of the repository.</typeparam>
public interface IAsyncRepository<TItem> : IRepository<TItem>
    where TItem : class
{
    Task<TItem> AddAsync(TItem item, CancellationToken cancellationToken = default);
    Task RemoveAsync(TItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync<TId>(TId id, TItem item, CancellationToken cancellationToken = default);
    Task<TItem?> GetAsync<TId>(TId id, CancellationToken cancellationToken = default);
}
