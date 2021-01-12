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
    public abstract class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> Table { get; private set; }
        private DbContext Context { get; set; }

        protected EntityRepository(DbContext context, DbSet<TEntity> table)
        {
            Context = context;
            Table = table;
        }

        public virtual Task<TEntity> Get(Guid id) => Table.FirstOrDefaultAsync(entity => entity.Id == id);

        public async virtual Task<IEnumerable<TEntity>> GetList(
            [Range(1, int.MaxValue)] int page,
            [Range(1, int.MaxValue)] int pageSize,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending)
        {
            if (page < 1)
                throw new ArgumentException($"{nameof(page)} was below 1. Received: {page}", nameof(page));
            if(pageSize < 1)
                throw new ArgumentException($"{nameof(pageSize)} was below 1. Received: {pageSize}", nameof(pageSize));

            IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

            // Pagination only skip if above page 1
            if (page > 1)
                query = query.Skip((page - 1) * pageSize);

            query = query.Take(pageSize);

            return await query.ToListAsync();
        }

        public async virtual Task<IEnumerable<TEntity>> GetList(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending)
        {
            IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy);

            return await query.ToListAsync();
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

            Table.Add(entity);

            return Task.CompletedTask;
        }

        public virtual Task Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Updated = DateTime.UtcNow;

            Table.Update(entity);

            return Task.CompletedTask;
        }

        public virtual Task<bool> Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Table.Remove(entity);

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

        protected IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending)
        {
            IQueryable<TEntity> query = Table;

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
}
