using System.Linq.Expressions;

using Simple.Data.Abstractions;
using Simple.Data.Abstractions.Repositories;

namespace Simple.Data.EntityFrameworkCore.Abstractions;

/// <summary>
/// Defines an <see cref="EntityFrameworkCore"/> specific extension of <see cref="IAsyncAuditableRepository{TAuditableEntity}"/>.
/// </summary>
/// <typeparam name="TAuditableEntity"></typeparam>
public interface IDbContextRepository<TAuditableEntity> : IAsyncAuditableRepository<TAuditableEntity>
    where TAuditableEntity : AuditableEntity
{
    IEnumerable<TAuditableEntity>? Find(Expression<Func<TAuditableEntity, bool>> predicate);
    Task<IEnumerable<TAuditableEntity>?> FindAsync(Expression<Func<TAuditableEntity, bool>> predicate, CancellationToken cancellationToken = default);
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
