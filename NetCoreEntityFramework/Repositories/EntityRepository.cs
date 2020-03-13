using Microsoft.EntityFrameworkCore;
using Nodes.NetCore.EntityFramework.Models;
using System;
using System.Threading.Tasks;

namespace Nodes.NetCore.EntityFramework.Repositories
{
    public abstract class EntityRepository<T, TContext> : IDisposable where T : EntityBase where TContext : DbContext
    {
        protected DbSet<T> Table { get; private set; }
        private TContext Context { get; set; }

        protected EntityRepository(TContext context, DbSet<T> table)
        {
            Context = context;
            Table = table;
        }

        public async Task<T> Get(Guid id)
        {
            return await Table.FirstOrDefaultAsync(entity => !entity.Deleted && entity.Id == id);
        }

        public void Dispose()
        {
            Context.SaveChanges();
        }
    }
}
