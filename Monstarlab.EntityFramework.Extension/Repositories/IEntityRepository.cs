namespace Monstarlab.EntityFramework.Extension.Repositories;

public interface IEntityRepository<TEntity> : IAsyncDisposable where TEntity : EntityBase
{
    /// <summary>
    /// Get the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to fetch.</param>
    Task<TEntity> Get(Guid id);

    /// <summary>
    /// Get multiple entities paginated.
    /// </summary>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Get multiple entities.
    /// </summary>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Get multiple entities paginated and translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Get multiple entities translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending);

    /// <summary>
    /// Add the given <paramref name="entity"/> to the database.
    /// An ID will be generated if not provided.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task Add(TEntity entity);

    /// <summary>
    /// Update the given <paramref name="entity"/> with the information set.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task Update(TEntity entity);

    /// <summary>
    /// Soft delete the <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">The entity to soft delete.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> Delete(TEntity entity);

    /// <summary>
    /// Delete the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to soft delete.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<bool> Delete(Guid id);
}
