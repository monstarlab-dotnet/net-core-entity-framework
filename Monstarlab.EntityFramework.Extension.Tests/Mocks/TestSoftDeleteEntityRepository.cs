namespace Nodes.NetCore.EntityFramework.Tests.Mocks;

public class TestSoftDeleteEntityRepository : EntitySoftDeleteRepository<TestSoftDeleteEntity>
{
    public TestSoftDeleteEntityRepository(TestContext context) : base(context)
    {
    }
}
