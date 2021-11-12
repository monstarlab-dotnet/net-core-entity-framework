namespace Monstarlab.EntityFramework.Extension.Repositories;

public abstract class BaseEntityRepository<TContext, TEntity, TId> : IBaseEntityRepository<TEntity, TId> where TEntity : EntityBase<TId> where TContext : DbContext
{
    protected TContext Context { get; }

    public BaseEntityRepository(TContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public virtual Task<TEntity> Get(TId id) => BaseIncludes().FirstOrDefaultAsync(entity => entity.Id.Equals(id));

    public async Task<TEntity> Add(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DateTime now = DateTime.UtcNow;

        entity.Created = now;
        entity.Updated = now;

        var addedEntity = Context.Set<TEntity>().Add(entity);

        await Context.SaveChangesAsync();

        return await Get(addedEntity.Entity.Id);
    }

    public virtual async Task Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.Updated = DateTime.UtcNow;

        Context.Set<TEntity>().Update(entity);

        await Context.SaveChangesAsync();
    }

    public virtual async Task<bool> Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        Context.Set<TEntity>().Remove(entity);

        await Context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<bool> Delete(TId id)
    {
        if (id.Equals(default(TId)))
            throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

        TEntity entity = await Get(id);

        if (entity == null)
            return false;

        return await Delete(entity);
    }

    protected IQueryable<TEntity> Paginate(IQueryable<TEntity> query, [Range(1, int.MaxValue)] int page, [Range(1, int.MaxValue)] int pageSize)
    {
        if (page < 1)
            throw new ArgumentException($"{nameof(page)} was below 1. Received: {page}", nameof(page));
        if (pageSize < 1)
            throw new ArgumentException($"{nameof(pageSize)} was below 1. Received: {pageSize}", nameof(pageSize));

        var q = query;

        // Pagination only skip if above page 1
        if (page > 1)
            q = q.Skip((page - 1) * pageSize);

        return q.Take(pageSize);
    }

    protected IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = BaseIncludes();

        if (where != null)
            query = query.Where(where);

        if (orderByExpression != null)
        {
            query = orderBy == OrderBy.Ascending
                ? query.OrderBy(orderByExpression)
                : query.OrderByDescending(orderByExpression);
        }

        return query;
    }

    /// <summary>
    /// Override this function to automatically include references in the result
    /// </summary>
    protected virtual IQueryable<TEntity> BaseIncludes() => Context.Set<TEntity>();
}
