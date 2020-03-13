using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nodes.NetCore.EntityFramework.Models
{
    public abstract class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Updated { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool Deleted { get; set; }

        public void Delete()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the database correctly
        /// </summary>
        /// <typeparam name="T">Should be same type as the model class</typeparam>
        /// <param name="table">The database table to delete the instance from</param>
        public void Delete<T>(DbSet<T> table) where T : EntityBase
        {
            if (typeof(T).FullName != GetType().FullName)
            {
                throw new ArgumentException($"Tried updating a table of type {typeof(T).FullName}. Expected type {GetType().FullName}", nameof(table));
            }

            Delete();

            table.Update(this as T);
        }

        public void Restore()
        {
            Deleted = false;
        }
    }
}
