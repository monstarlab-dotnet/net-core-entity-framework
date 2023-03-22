namespace Monstarlab.EntityFramework.Extension.Repositories;

public class EntityRepository<TContext, TEntity, TId> : BaseEntityRepository<TContext, TEntity, TId>, IEntityRepository<TEntity, TId> 
    where TContext : DbContext 
    where TEntity : EntityBase<TId>
{
    public EntityRepository(TContext context) : base(context) { }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> where) 
        => BaseIncludes().FirstOrDefaultAsync(where);

    public virtual Task<ListWrapper<TEntity>> GetListAsync(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        var query = GetQueryable(where, orderByExpression, orderBy);

        return GetListAsync(query, page, pageSize);
    }

    public virtual Task<ListWrapper<TResult>> GetListWithSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        var query = GetQueryable(where, orderByExpression, orderBy);

        var selectedQuery = query.Select(select);

        return GetListAsync(selectedQuery, page, pageSize);
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        var query = GetQueryable(where, orderByExpression, orderBy);

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<TResult>> GetListWithSelectAsync<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        var query = GetQueryable(where, orderByExpression, orderBy);

        return await query.Select(select).ToListAsync();
    }
}
