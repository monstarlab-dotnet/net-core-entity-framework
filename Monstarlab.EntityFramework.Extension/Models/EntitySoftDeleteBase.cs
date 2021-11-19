namespace Monstarlab.EntityFramework.Extension.Models;

public abstract class EntitySoftDeleteBase<TId> : EntityBase<TId>
{
    [ReadOnly(true)]
    public DateTime? DeletedAt { get; set; }

    [ReadOnly(true)]
    public bool Deleted { get; set; }
}
