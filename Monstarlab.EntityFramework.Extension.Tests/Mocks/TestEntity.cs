using System.ComponentModel;

namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestEntity : EntityBase<Guid>
{
    public string Property { get; set; }

    [ReadOnly(true)]
    public string ReadOnlyProperty { get; set; }
    
    public virtual IEnumerable<TestSubEntity> TestSubEntities { get; set; }
    
    public virtual SingleTestSubEntity TestSubEntity { get; set; }
}

public class TestSubEntity : EntityBase<Guid>
{
    public string Property { get; set; }
    public virtual TestEntity TestEntity { get; set; }
}

public class SingleTestSubEntity : EntityBase<Guid>
{
    public string Property { get; set; }
    public virtual TestEntity TestEntity { get; set; }
}