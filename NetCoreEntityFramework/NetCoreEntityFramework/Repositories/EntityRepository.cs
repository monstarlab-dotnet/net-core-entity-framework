using Microsoft.EntityFrameworkCore;
using NetCoreEntityFramework.Models;
using System;
using System.Threading.Tasks;

namespace NetCoreEntityFramework.Repositories
{
    public abstract class EntityRepository<T> where T : EntityBase
    {
        protected DbSet<T> Table { get; private set; }

        protected EntityRepository(DbSet<T> table)
        {
            Table = table;
        }

        public async Task<T> Get(Guid id)
        {
            return await Table.FirstOrDefaultAsync(entity => !entity.Deleted && entity.Id == id);
        }
    }
}
