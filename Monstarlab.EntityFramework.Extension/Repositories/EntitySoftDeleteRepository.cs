namespace Monstarlab.EntityFramework.Extension.Repositories;

public class EntitySoftDeleteRepository<TContext, TEntity, TId> : BaseEntityRepository<TContext, TEntity, TId>, IEntitySoftDeleteRepository<TEntity, TId> where TContext : DbContext where TEntity : EntitySoftDeleteBase<TId>
{
    public EntitySoftDeleteRepository(TContext context) : base(context)
    {
    }

    public virtual Task<TEntity> Get(TId id, bool includeDeleted = false) => BaseIncludes().FirstOrDefaultAsync(entity => (includeDeleted || !entity.Deleted) && entity.Id.Equals(id));

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

    public virtual async Task<bool> Restore(TId id)
    {
        if (id.Equals(default(TId)))
            throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

        TEntity entity = await Get(id, true);

        if (entity == null)
            return false;

        return await Restore(entity);
    }

    public virtual async Task<bool> Restore(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.Deleted = false;
        entity.DeletedAt = null;

        Context.Set<TEntity>().Update(entity);

        await Context.SaveChangesAsync();

        return true;
    }

    protected IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>> where = null,
        Expression<Func<TEntity, object>> orderByExpression = null,
        OrderBy orderBy = OrderBy.Ascending,
        GetListMode mode = GetListMode.ExcludeDeleted)
    {
        var query = base.GetQueryable(where, orderByExpression, orderBy);

        query = mode switch
        {
            GetListMode.ExcludeDeleted => query.Where(e => !e.Deleted),
            GetListMode.OnlyDeleted => query.Where(e => e.Deleted),
            GetListMode.IncludeDeleted => query,
            _ => throw new ArgumentException("Unknown setting", nameof(mode))
        };

        return query;
    }

    public async Task Update(TEntity entity, GetListMode mode = GetListMode.ExcludeDeleted)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (mode == GetListMode.IncludeDeleted)
        {
            await base.Update(entity);
            return;
        }

        else if (mode == GetListMode.ExcludeDeleted)
        {
            if (!await IsDeleted(entity.Id))
                await base.Update(entity);

            return;
        }

        else if (mode == GetListMode.OnlyDeleted)
        {
            if (await IsDeleted(entity.Id))
                await base.Update(entity);

            return;
        }
    }

    /// <summary>
    /// Check if the entity with the given <paramref name="id"/> is deleted or not
    /// </summary>
    /// <param name="id">The ID of the entity to check</param>
    protected async Task<bool> IsDeleted(TId id)
    {
        var entity = await Context.Set<TEntity>().Select(e => new { e.Id, e.Deleted }).FirstOrDefaultAsync(e => e.Id.Equals(id));

        return entity?.Deleted ?? true;
    }
}
