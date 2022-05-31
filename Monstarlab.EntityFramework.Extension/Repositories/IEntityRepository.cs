namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IEntityRepository<TEntity, TId> : IBaseEntityRepository<TEntity, TId> 
    where TEntity : EntityBase<TId>
{
    /// <summary>
    /// Get the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to fetch.</param>
    Task<TEntity> GetAsync(TId id);

    /// <summary>
    /// Get multiple entities paginated.
    /// </summary>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<ListWrapper<TEntity>> GetListAsync(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Get multiple entities.
    /// </summary>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

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
    /// <exception cref="ArgumentException"></exception>
    Task<ListWrapper<TResult>> GetListWithSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Get multiple entities translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="where">The filter expressions.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    Task<IEnumerable<TResult>> GetListWithSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Update the given <paramref name="entity"/> with the information set.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<TEntity> UpdateAsync(TEntity entity);
}
