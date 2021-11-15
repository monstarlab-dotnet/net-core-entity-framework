namespace Monstarlab.EntityFramework.Extension.Repositories;

public class EntityRepository<TContext, TEntity, TId> : BaseEntityRepository<TContext, TEntity, TId>, IBaseEntityRepository<TEntity, TId> where TContext : DbContext where TEntity : EntityBase<TId>
{
    public EntityRepository(TContext context) : base(context) { }

    public virtual Task<ListWrapper<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        return GetListAsync(query, page, pageSize);
    }

    public virtual Task<ListWrapper<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        var selectedQuery = query.Select(select);

        return GetListAsync(selectedQuery, page, pageSize);
    }

    public async virtual Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        return await query.ToListAsync();
    }

    public async virtual Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        return await query.Select(select).ToListAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.SaveChangesAsync();
    }
}
