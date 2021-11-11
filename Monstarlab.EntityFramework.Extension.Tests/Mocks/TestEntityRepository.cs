namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestEntityRepository : EntityRepository<TestEntity>
{
    public TestEntityRepository(TestContext context) : base(context)
    {
    }
}
