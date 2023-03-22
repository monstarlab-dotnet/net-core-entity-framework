namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestSubEntityRepository : EntityRepository<TestContext, TestSubEntity, Guid>
{
    public TestSubEntityRepository(TestContext context) : base(context)
    {
    }
}

public class SingleTestSubEntityRepository : EntityRepository<TestContext, SingleTestSubEntity, Guid>
{
    public SingleTestSubEntityRepository(TestContext context) : base(context)
    {
    }
}