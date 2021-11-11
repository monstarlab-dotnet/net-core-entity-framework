namespace Monstarlab.EntityFramework.Extension.Repositories;

public class EntitySoftDeleteRepository<TEntity> : EntityRepository<TEntity>, IEntitySoftDeleteRepository<TEntity> where TEntity : EntitySoftDeleteBase
{
    public EntitySoftDeleteRepository(DbContext context) : base(context)
    {
    }

    public virtual Task<TEntity> Get(Guid id, bool includeDeleted = false) => BaseIncludes().FirstOrDefaultAsync(entity => (includeDeleted || !entity.Deleted) && entity.Id == id);

    public async virtual Task<IEnumerable<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

        query = Paginate(query, page, pageSize);

        return await query.ToListAsync();
    }

    public async virtual Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<TResult>> GetListWithSelect<TResult>(Expression<Func<TEntity, TResult>> select,
                                                                    [Range(1, int.MaxValue)] int page,
                                                                    [Range(1, int.MaxValue)] int pageSize,
                                                                    Expression<Func<TEntity, bool>> where = null,
                                                                    Expression<Func<TEntity, object>> orderByExpression = null,
                                                                    OrderBy orderBy = OrderBy.Ascending,
                                                                    GetListMode mode = GetListMode.ExcludeDeleted)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

        query = Paginate(query, page, pageSize);

        return await query.Select(select).ToListAsync();
    }

    public async virtual Task<IEnumerable<TResult>> GetListWithSelect<TResult>(Expression<Func<TEntity, TResult>> select,
                                                                    Expression<Func<TEntity, bool>> where = null,
                                                                    Expression<Func<TEntity, object>> orderByExpression = null,
                                                                    OrderBy orderBy = OrderBy.Ascending,
                                                                    GetListMode mode = GetListMode.ExcludeDeleted)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

        return await query.Select(select).ToListAsync();
    }

    public override Task<bool> Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.DeletedAt = DateTime.UtcNow;
        entity.Deleted = true;

        Context.Set<TEntity>().Update(entity);

        return Task.FromResult(true);
    }

    public virtual async Task<bool> Restore(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

        TEntity entity = await Get(id, true);

        if (entity == null)
            return false;

        return await Restore(entity);
    }

    public virtual Task<bool> Restore(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.Deleted = false;
        entity.DeletedAt = null;

        Context.Set<TEntity>().Update(entity);

        return Task.FromResult(true);
    }

    protected IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted)
    {
        var query = base.GetQueryable(where, orderByExpression, orderBy);

        switch(mode)
        {
            case GetListMode.ExcludeDeleted:
                query = query.Where(e => !e.Deleted);
                break;
            case GetListMode.OnlyDeleted:
                query = query.Where(e => e.Deleted);
                break;
            //Do nothing if everything should be included
            case GetListMode.IncludeDeleted:
                break;
            default:
                throw new ArgumentException("Unknown setting", nameof(mode));
        }

        return query;
    }
}
