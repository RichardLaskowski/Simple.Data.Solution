namespace Simple.Data.Abstractions.Repositories;

/// <summary>
/// Defines an <see cref="IRepository{TItem}"/> of <see cref="AuditableEntity"/>
/// </summary>
/// <typeparam name="TAuditableEntity"></typeparam>
public interface IAuditableRepository<TAuditableEntity> : IRepository<TAuditableEntity>
    where TAuditableEntity : AuditableEntity
{
}
