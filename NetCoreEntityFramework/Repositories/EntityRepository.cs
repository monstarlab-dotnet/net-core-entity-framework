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
    public abstract class EntityRepository<TEntity, TContext> : IDisposable where TEntity : EntityBase where TContext : DbContext
    {
        protected DbSet<TEntity> Table { get; private set; }
        private TContext Context { get; set; }

        protected EntityRepository(TContext context, DbSet<TEntity> table)
        {
            Context = context;
            Table = table;
        }

        /// <summary>
        /// Get the entity with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the entity to fetch.</param>
        /// <param name="includeDeleted">If true, also search amongst the soft deleted entities.</param>
        public async Task<TEntity> Get(Guid id, bool includeDeleted = false)
        {
            return await Table.FirstOrDefaultAsync(entity => (includeDeleted || !entity.Deleted) && entity.Id == id);
        }

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
        public async Task<IEnumerable<TEntity>> GetList<TKey>(
            [Range(1, int.MaxValue)] int page,
            [Range(1, int.MaxValue)] int pageSize,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TKey>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending,
            GetListMode mode = GetListMode.ExcludeDeleted)
        {
            if (page < 1)
                throw new ArgumentException($"{nameof(page)} was below 1. Received: {page}", nameof(page));
            if(pageSize < 1)
                throw new ArgumentException($"{nameof(pageSize)} was below 1. Received: {pageSize}", nameof(pageSize));

            IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

            // Pagination only skip if above page 1
            if (page > 1)
                query = query.Skip((page - 1) * pageSize);

            query = query.Take(pageSize);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get multiple entities.
        /// </summary>
        /// <param name="where">The filter expression.</param>
        /// <param name="orderByExpression">The expression to order by.</param>
        /// <param name="orderBy">To order by ascending or descending.</param>
        /// <param name="mode">Whether to include deleted or not.</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<TEntity>> GetList<TKey>(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TKey>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending,
            GetListMode mode = GetListMode.ExcludeDeleted)
        {
            IQueryable<TEntity> query = GetQueryable(where, orderByExpression, orderBy, mode);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Add the given <paramref name="entity"/> to the database.
        /// An ID will be generated if not provided.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<TEntity> Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            DateTime now = DateTime.UtcNow;

            entity.Created = now;
            entity.Updated = now;

            Context.Add(entity);

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Update the given <paramref name="entity"/> with the information set.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<TEntity> Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Updated = DateTime.UtcNow;

            Context.Update(entity);

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Soft delete the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.DeletedAt = DateTime.UtcNow;
            entity.Deleted = true;

            Context.Update(entity);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Soft the delete the entity with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the entity to soft delete.</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

            TEntity entity = await Get(id);

            if (entity == null)
                return false;

            return await Delete(entity);
        }

        /// <summary>
        /// Restore/undelete the entity with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of the entity to restore.</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> Restore(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{nameof(id)} was not set", nameof(id));

            TEntity entity = await Get(id, true);

            if (entity == null)
                return false;

            return await Restore(entity);
        }

        /// <summary>
        /// Restore/undelete the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The entity to restore.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> Restore(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Deleted = false;

            Context.Update(entity);

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            Context.SaveChanges();
        }

        private IQueryable<TEntity> GetQueryable<TKey>(
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TKey>> orderByExpression = null,
            OrderBy orderBy = OrderBy.Ascending,
            GetListMode mode = GetListMode.ExcludeDeleted)
        {
            IQueryable<TEntity> query;

            switch (mode)
            {
                case GetListMode.ExcludeDeleted:
                    query = Table.Where(e => !e.Deleted);
                    break;
                case GetListMode.IncludeDeleted:
                    query = Table;
                    break;
                case GetListMode.OnlyDeleted:
                    query = Table.Where(e => e.Deleted);
                    break;
                default:
                    throw new ArgumentException("Unknown setting", nameof(mode));
            }

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
    }
}
