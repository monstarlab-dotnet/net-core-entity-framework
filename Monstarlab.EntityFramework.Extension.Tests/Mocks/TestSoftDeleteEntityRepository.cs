namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestSoftDeleteEntityRepository : EntitySoftDeleteRepository<TestContext, TestSoftDeleteEntity, Guid>
{
    public TestSoftDeleteEntityRepository(TestContext context) : base(context)
    {
    }
}
