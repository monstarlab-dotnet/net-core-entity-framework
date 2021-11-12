namespace Monstarlab.EntityFramework.Extension.Models;

public abstract class EntitySoftDeleteBase<TId> : EntityBase<TId>
{
    public DateTime? DeletedAt { get; set; }

    public bool Deleted { get; set; }
}
