namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestSoftDeleteEntity : EntitySoftDeleteBase<Guid>
{
    public string Property { get; set; }
}
