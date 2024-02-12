namespace Simple.Data.Abstractions.Repositories;

/// <summary>
/// Defines an extension of <see cref="IAuditableRepository{TAuditableEntity}"/> that adds <see langword="async"/> methods.
/// </summary>
/// <typeparam name="TAuditableEntity">The <see cref="AuditableEntity"/> items of the repository.</typeparam>
public interface IAsyncAuditableRepository<TAuditableEntity> : IAsyncRepository<TAuditableEntity>
    where TAuditableEntity : AuditableEntity
{
}
