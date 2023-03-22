using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monstarlab.EntityFramework.Extension.Tests.Mocks;

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasOne(e => e.TestSubEntity)
            .WithOne(e => e.TestEntity).HasForeignKey(nameof(SingleTestSubEntity));
    }
}