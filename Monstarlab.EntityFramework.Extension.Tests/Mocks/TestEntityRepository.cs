namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestEntityRepository : EntityRepository<TestEntity, Guid>
{
    public TestEntityRepository(TestContext context) : base(context)
    {
    }
}
