namespace Monstarlab.EntityFramework.Extension.Repositories;

public abstract class BaseEntityRepository<TContext, TEntity, TId> : IBaseEntityRepository<TEntity, TId> where TEntity : EntityBase<TId> where TContext : DbContext
{
    protected TContext Context { get; }

    public BaseEntityRepository(TContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public virtual Task<TEntity> GetAsync(TId id) => BaseIncludes().FirstOrDefaultAsync(entity => entity.Id.Equals(id));

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DateTime now = DateTime.UtcNow;

        entity.Created = now;
        entity.Updated = now;

        var addedEntity = await Context.Set<TEntity>().AddAsync(entity);

        //await Context.SaveChangesAsync();

        return addedEntity.Entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var originalEntity = await GetAsync(entity.Id);

        if (originalEntity == null)
            throw new ArgumentException($"No entity found with the id {entity.Id}");

        originalEntity.Updated = DateTime.UtcNow;

        foreach (PropertyInfo prop in originalEntity.GetType().GetProperties())
        {
            if (prop.CanWrite)
            {
                var initialValue = prop.GetValue(originalEntity);
                var potentialNewValue = prop.GetValue(entity);

                if (potentialNewValue != null && potentialNewValue != initialValue && !PropertyIsReadOnly(prop))
                    prop.SetValue(originalEntity, potentialNewValue);
            }
        }

        var updatedEntity = Context.Set<TEntity>().Update(originalEntity);

        //await Context.SaveChangesAsync();

        return await GetAsync(updatedEntity.Entity.Id);
    }

    private bool PropertyIsReadOnly(PropertyInfo prop) => (prop.GetCustomAttribute(typeof(ReadOnlyAttribute), true) as ReadOnlyAttribute)?.IsReadOnly ?? false;

    public virtual async Task<bool> DeleteAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        Context.Set<TEntity>().Remove(entity);

        //await Context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<bool> DeleteAsync(TId id)
    {
        if (id.Equals(default(TId)))
            throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

        TEntity entity = await GetAsync(id);

        if (entity == null)
            return false;

        return await DeleteAsync(entity);
    }

    protected IQueryable<T> Paginate<T>(IQueryable<T> query, [Range(1, int.MaxValue)] int page, [Range(1, int.MaxValue)] int pageSize)
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
        Expression<Func<TEntity, bool>>[] where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending)
    {
        IQueryable<TEntity> query = BaseIncludes();

        if (where != null && where.Any())
        {
            foreach(var w in where)
            {
                query = query.Where(w);
            }
        }

        if (orderByExpression != null)
        {
            query = orderBy == OrderBy.Ascending
                ? query.OrderBy(orderByExpression)
                : query.OrderByDescending(orderByExpression);
        }

        return query;
    }

    protected async Task<ListWrapper<T>> GetListAsync<T>(IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();

        var paginatedQuery = Paginate(query, page, pageSize);

        var data = await paginatedQuery.ToListAsync();

        return new ListWrapper<T>
        {
            Data = data,
            Meta = new MetaData
            {
                CurrentPage = page,
                PerPage = pageSize,
                RecordsInDataset = data.Count,
                Total = totalCount
            }
        };
    }

    /// <summary>
    /// Override this function to automatically include references in the result
    /// </summary>
    protected virtual IQueryable<TEntity> BaseIncludes() => Context.Set<TEntity>();
}
