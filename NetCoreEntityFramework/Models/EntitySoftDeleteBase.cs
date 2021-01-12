using System;

namespace Nodes.NetCore.EntityFramework.Models
{
    public abstract class EntitySoftDeleteBase : EntityBase
    {
        public DateTime? DeletedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
