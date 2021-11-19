namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IBaseEntityRepository<TEntity, TId> where TEntity : EntityBase<TId>
{
    /// <summary>
    /// Add the given <paramref name="entity"/> to the database.
    /// An ID will be generated if not provided.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Soft delete the <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">The entity to soft delete.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> DeleteAsync(TEntity entity);

    /// <summary>
    /// Delete the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to soft delete.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<bool> DeleteAsync(TId id);
}
