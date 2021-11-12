using System.ComponentModel;

namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestEntity : EntityBase<Guid>
{
    public string Property { get; set; }

    [ReadOnly(true)]
    public string ReadOnlyProperty { get; set; }
}
