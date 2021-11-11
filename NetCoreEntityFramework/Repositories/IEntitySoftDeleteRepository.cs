using Nodes.NetCore.EntityFramework.Enums;
using Nodes.NetCore.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nodes.NetCore.EntityFramework.Repositories;

public interface IEntitySoftDeleteRepository<TEntity> : IEntityRepository<TEntity> where TEntity : EntitySoftDeleteBase
{
    /// <summary>
    /// Get the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to fetch.</param>
    /// <param name="includeDeleted">If true, also search amongst the soft deleted entities.</param>
    Task<TEntity> Get(Guid id, bool includeDeleted = false);

    /// <summary>
    /// Get multiple entities paginated.
    /// </summary>
    /// <param name="page">Which page to fetch (1 and above).</param>
    /// <param name="pageSize">The size of each page (1 and above).</param>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities.
    /// </summary>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>> where = null,
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
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Get multiple entities translated.
    /// </summary>
    /// <typeparam name="TResult">The type to return.</typeparam>
    /// <param name="select">The select statement to get <typeparamref name="TResult"/>.</param>
    /// <param name="where">The filter expression.</param>
    /// <param name="orderByExpression">The expression to order by.</param>
    /// <param name="orderBy">To order by ascending or descending.</param>
    /// <param name="mode">Whether to include deleted or not.</param>
    Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted);

    /// <summary>
    /// Restore/undelete the entity with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the entity to restore.</param>
    /// <exception cref="ArgumentException"></exception>
    Task<bool> Restore(Guid id);

    /// <summary>
    /// Restore/undelete the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">The entity to restore.</param>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> Restore(TEntity entity);
}
