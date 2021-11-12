namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IEntitySoftDeleteRepository<TEntity, TId> : IBaseEntityRepository<TEntity, TId> where TEntity : EntitySoftDeleteBase<TId>
{
    /// <summary>
    /// Get the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to fetch.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    Task<TEntity> Get(TId id, GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities paginated.
    /// </summary>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities.
    /// </summary>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities paginated and translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Update the given <paramref name="entity"/> with the information set.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="mode">Whether to include deleted or not to be able to be updated</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<TEntity> Update(TEntity entity, GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Restore/undelete the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to restore.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<TEntity> Restore(TId id);

    /// <summary>
    /// Restore/undelete the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">The entity to restore.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<TEntity> Restore(TEntity entity);
}
