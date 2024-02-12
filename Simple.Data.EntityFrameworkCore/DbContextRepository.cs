using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Diagnostics;

using Microsoft.EntityFrameworkCore;

using Simple.Data.Abstractions;
using Simple.Data.EntityFrameworkCore.Abstractions;

namespace Simple.Data.EntityFrameworkCore;

/// <summary>
/// An <see cref="EntityFrameworkCore"/> implementation of the <see cref="IAsyncAuditableRepository{TAuditableEntity}"/> interface.
/// </summary>
/// <typeparam name="TAuditableEntity"></typeparam>
public class DbContextRepository<TAuditableEntity> : IDbContextRepository<TAuditableEntity>
    where TAuditableEntity : AuditableEntity
{
    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditableDbContext"></param>
    public DbContextRepository(AuditableDbContext auditableDbContext)
    {
        Guard.IsNotNull<AuditableDbContext>(auditableDbContext);

        this.auditableDbContext = auditableDbContext;
        auditableEntities = auditableDbContext.Set<TAuditableEntity>();
    }

    #endregion Constructors


    #region Public Methods

    /// <summary>
    /// Begins tracking the given entity, and any other reachable entities that are not already being tracked, in the Added state such that they will be inserted into the database when SaveChanges() is called.
    /// </summary>
    /// <param name="auditableEntity">The entity to add.</param>
    /// <returns>The entity.</returns>
    public TAuditableEntity Add(TAuditableEntity auditableEntity)
    {
        return auditableEntities.Add(auditableEntity).Entity;
    }


    /// <summary>
    /// Begins tracking the given entity, and any other reachable entities that are not already being tracked, in the Added state such that they will be inserted into the database when SaveChanges() is called.
    /// </summary>
    /// <param name="auditableEntity">The entity to add.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous Add operation. The task result contains the Entity for the entity.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    public async Task<TAuditableEntity> AddAsync(TAuditableEntity auditableEntity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return (await auditableEntities.AddAsync(auditableEntity, cancellationToken)).Entity;
    }


    /// <summary>
    /// Finds an entity using the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">The search criteria.</param>
    /// <returns>The entity found, or null.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<TAuditableEntity>? Find(Expression<Func<TAuditableEntity, bool>> predicate)
    {
        return auditableEntities.Where(predicate).ToList();
    }


    /// <summary>
    /// Find an entity using the given <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">The search criteria</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or null.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    public async Task<IEnumerable<TAuditableEntity>?> FindAsync(Expression<Func<TAuditableEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await auditableEntities.Where(predicate).ToListAsync(cancellationToken);
    }


    /// <summary>
    /// Gets an entity with the given primary key value.
    /// </summary>
    /// <typeparam name="TId">The primary key <see cref="Type"/>"/></typeparam>
    /// <param name="id">The primary key of the entity.</param>
    /// <returns>The entity found, or null</returns>
    public TAuditableEntity? Get<TId>(TId id)
    {
        return auditableEntities.Find(id);
    }


    /// <summary>
    /// Gets an entity with the given primary key value.
    /// </summary>
    /// <typeparam name="TId">The primary key <see cref="Type"/>"/></typeparam>
    /// <param name="id">The primary key of the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or null</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    public async Task<TAuditableEntity?> GetAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await auditableEntities.FindAsync(new object?[] { id }, cancellationToken);
    }


    /// <summary>
    /// Begins tracking the given entity in the Deleted state.
    /// </summary>
    /// <param name="auditableEntity">The entity to remove.</param>
    public void Remove(TAuditableEntity auditableEntity)
    {
        auditableEntities.Remove(auditableEntity);
    }


    /// <summary>
    /// Begins tracking the given entity in the Deleted state.
    /// </summary>
    /// <param name="auditableEntity">The entity to remove.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronouse operation.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    /// <exception cref=" ArgumentNullException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task RemoveAsync(TAuditableEntity auditableEntity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Run(() => auditableEntities.Remove(auditableEntity), cancellationToken);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    public int SaveChanges()
    {
        return auditableDbContext.SaveChanges();
    }


    /// <summary>
    /// Saves all changes made in this repository to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await auditableDbContext.SaveChangesAsync(cancellationToken);
    }


    /// <summary>
    /// Begins tracking the given entity and entries reachable from the given entity using the EntityState.Modified state by default.
    /// </summary>
    /// <typeparam name="TId">The <see cref="Type"/> of <paramref name="id"/>.</typeparam>
    /// <param name="id">The primary key of the entity.</param>
    /// <param name="auditableEntity">The entity updated.</param>
    public void Update<TId>(TId id, TAuditableEntity auditableEntity)
    {
        auditableEntities.Update(auditableEntity);
    }


    /// <summary>
    /// Begins tracking the given entity and entries reachable from the given entity using the EntityState.Modified state by default.
    /// </summary>
    /// <typeparam name="TId">The <see cref="Type"/> of <paramref name="id"/>.</typeparam>
    /// <param name="id">The primary key of the entity.</param>
    /// <param name="auditableEntity">The entity updated.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The entity updated.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken"/> is canceled.</exception>
    /// <exception cref=" ArgumentNullException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public async Task UpdateAsync<TId>(TId id, TAuditableEntity auditableEntity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Run(() => auditableEntities.Remove(auditableEntity), cancellationToken);
    }

    #endregion Public Methods


    #region Fields

    /// <summary>
    /// 
    /// </summary>
    protected readonly DbSet<TAuditableEntity> auditableEntities;
    /// <summary>
    /// 
    /// </summary>
    protected readonly AuditableDbContext auditableDbContext;

    #endregion Fields
}

