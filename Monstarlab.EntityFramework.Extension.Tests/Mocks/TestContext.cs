﻿namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options) { }

    public DbSet<TestEntity> Table { get; set; }

    public DbSet<TestSoftDeleteEntity> SoftDeleteTable { get; set; }
}
