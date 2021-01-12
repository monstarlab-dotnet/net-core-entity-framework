using Microsoft.EntityFrameworkCore;
using Nodes.NetCore.EntityFramework.Enums;
using Nodes.NetCore.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nodes.NetCore.EntityFramework.Repositories
{
    public abstract class EntitySoftDeleteRepository<TEntity> : EntityRepository<TEntity>, IEntitySoftDeleteRepository<TEntity> where TEntity : EntitySoftDeleteBase
    {
        protected EntitySoftDeleteRepository(DbContext context, DbSet<TEntity> table) : base(context, table)
        {
        }

        public override Task<TEntity> Get(Guid id) => Get(id, false);

        public Task<TEntity> Get(Guid id, bool includeDeleted = false) => Table.FirstOrDefaultAsync(entity => (includeDeleted || !entity.Deleted) && entity.Id == id);

        public override Task<IEnumerable<TEntity>> GetList(
            [Range(1, int.MaxValue)] int page,
            [Range(1, int.MaxValue)] int pageSize,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending) => GetList(page, pageSize, where, orderByExpression, orderBy, GetListMode.ExcludeDeleted);

        public override Task<IEnumerable<TEntity>> GetList(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending) => GetList(where, orderByExpression, orderBy, GetListMode.ExcludeDeleted);

        public async virtual Task<IEnumerable<TEntity>> GetList(
            [Range(1, int.MaxValue)] int page,
            [Range(1, int.MaxValue)] int pageSize,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending,
            GetListMode mode = GetListMode.ExcludeDeleted)
        {
            if (page < 1)
                throw new ArgumentException($"{nameof(page)} was below 1. Received: {page}", nameof(page));
            if (pageSize < 1)
                throw new ArgumentException($"{nameof(pageSize)} was below 1. Received: {pageSize}", nameof(pageSize));

            IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

            // Pagination only skip if above page 1
            if (page > 1)
                query = query.Skip((page - 1) * pageSize);

            query = query.Take(pageSize);

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

        public override Task<bool> Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.DeletedAt = DateTime.UtcNow;
            entity.Deleted = true;

            Table.Update(entity);

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

            Table.Update(entity);

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
}
