namespace Monstarlab.EntityFramework.Extension.Repositories;

public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : EntityBase
{
    protected DbContext Context { get; }

    public EntityRepository(DbContext context)
    {
        Context = context;
    }

    public virtual Task<TEntity> Get(Guid id) => BaseIncludes().FirstOrDefaultAsync(entity => entity.Id == id);

    public async virtual Task<IEnumerable<TEntity>> GetList(
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        query = Paginate(query, page, pageSize);

        return await query.ToListAsync();
    }

    public async virtual Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        [Range(1, int.MaxValue)] int page,
        [Range(1, int.MaxValue)] int pageSize,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        query = Paginate(query, page, pageSize);

        return await query.Select(select).ToListAsync();
    }

    public async virtual Task<IEnumerable<TEntity>> GetList(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        return await query.ToListAsync();
    }

    public async virtual Task<IEnumerable<TResult>> GetListWithSelect<TResult>(
        Expression<Func<TEntity, TResult>> select,
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

        return await query.Select(select).ToListAsync();
    }

    public virtual Task Add(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        DateTime now = DateTime.UtcNow;

        entity.Created = now;
        entity.Updated = now;

        Context.Set<TEntity>().Add(entity);

        return Task.CompletedTask;
    }

    public virtual Task Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.Updated = DateTime.UtcNow;

        Context.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }

    public virtual Task<bool> Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        Context.Set<TEntity>().Remove(entity);

        return Task.FromResult(true);
    }

    public virtual async Task<bool> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

        TEntity entity = await Get(id);

        if (entity == null)
            return false;

        return await Delete(entity);
    }

    /// <summary>
    /// Override this function to automatically include references in the result
    /// </summary>
    protected virtual IQueryable<TEntity> BaseIncludes() => Context.Set<TEntity>();

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

    public async ValueTask DisposeAsync()
    {
        await Context.SaveChangesAsync();
    }
}
