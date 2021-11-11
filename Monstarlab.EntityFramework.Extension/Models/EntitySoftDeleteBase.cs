namespace Monstarlab.EntityFramework.Extension.Models;

public abstract class EntitySoftDeleteBase : EntityBase
{
    public DateTime? DeletedAt { get; set; }

    public bool Deleted { get; set; }
}
