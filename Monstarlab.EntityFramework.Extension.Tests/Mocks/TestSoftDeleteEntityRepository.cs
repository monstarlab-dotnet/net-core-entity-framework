namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestSoftDeleteEntityRepository : EntitySoftDeleteRepository<TestSoftDeleteEntity, Guid>
{
    public TestSoftDeleteEntityRepository(TestContext context) : base(context)
    {
    }
}
