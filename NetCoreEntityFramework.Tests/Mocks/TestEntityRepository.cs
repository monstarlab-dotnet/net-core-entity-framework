using Nodes.NetCore.EntityFramework.Repositories;

namespace Nodes.NetCore.EntityFramework.Tests.Mocks;

public class TestEntityRepository : EntityRepository<TestEntity>
{
    public TestEntityRepository(TestContext context) : base(context)
    {
    }
}
