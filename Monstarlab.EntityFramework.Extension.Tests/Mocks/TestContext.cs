using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestContext : DbContext
{
    public TestContext(DbContextOptions options) : base(options) { }

    public DbSet<TestEntity> Table { get; set; }
    
    public DbSet<TestSubEntity> SubEntityTable { get; set; }
    public DbSet<SingleTestSubEntity> SingleSubEntityTable { get; set; }

    public DbSet<TestSoftDeleteEntity> SoftDeleteTable { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    
    
}

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasOne(e => e.TestSubEntity)
            .WithOne(e => e.TestEntity).HasForeignKey(nameof(SingleTestSubEntity));
    }
}