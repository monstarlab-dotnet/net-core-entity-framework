using FluentAssertions;

namespace Monstarlab.EntityFramework.Extension.Tests;

public class EntityRepositoryTests
{
    private TestEntityRepository _repository;
    private TestContext _context;
    private TestEntity _entity;
    private IUnitOfWork _unitOfWork;
    private IEnumerable<TestEntity> _listEntities;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _context = new TestContext(options);

        _repository = new TestEntityRepository(_context);

        var now = DateTime.UtcNow;

        _entity = new TestEntity
        {
            Created = now,
            Id = Guid.NewGuid(),
            Updated = now,
            Property = string.Empty,
            ReadOnlyProperty = "I'm readonly"
        };

        _context.Table.Add(_entity);

        _listEntities = GetTestList();
        _context.Table.AddRange(_listEntities);

        _context.SaveChanges();

        _unitOfWork = new UnitOfWork<TestContext>(_context);

        _repository = new TestEntityRepository(_context);
    }

    #region Add
    [Test]
    public async Task AddAddsEntityAndSetsAttributes()
    {
        int startSize = await _context.Table.CountAsync();
        int expectedSize = startSize + 1;
        var entity = new TestEntity();

        
        var addedEntity = await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();

        
        addedEntity.Id.Should().NotBeEmpty();
        addedEntity.Created.Should().NotBe(default);
        addedEntity.Updated.Should().NotBe(default);
        (await _context.Table.CountAsync()).Should().Be(expectedSize);
    }

    [Test]
    [AutoData]
    public async Task AddEntityWithIdKeepsId(Guid idToCreate)
    {
        var entity = new TestEntity
        {
            Id = idToCreate
        };

        
        var addedEntity = await _repository.AddAsync(entity);
        await _unitOfWork.CommitAsync();

        
        addedEntity.Id.Should().Be(idToCreate);
    }

    [Test]
    public Task AddThrowsExceptionIfEntityIsNull()
    {
        return _repository
            .Awaiting(r => r.AddAsync(null))
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }
    #endregion

    #region List
    [Test]
    public async Task GetListReturnsAll()
    {
        var entities = await _repository.GetListAsync(null, null, OrderBy.Ascending);

        
        entities.Should().HaveCount(_listEntities.Count() + 1);
    }

    [Test]
    public async Task GetListWhere()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property == "b"
        });

        
        entities.Should().HaveCount(1);
        entities.ElementAt(0).Should().BeSameAs(_listEntities.First(x => x.Property == "b"));
    }

    [Test]
    public async Task GetListOrderBy()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property.Length == 1
        }, x => x.Property);

        
        entities.Should()
            .HaveSameCount(_listEntities)
            .And
            .ContainInOrder(_listEntities.OrderBy(e => e.Property));
    }

    [Test]
    public async Task GetListOrderByDescending()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property.Length == 1
        }, x => x.Property, OrderBy.Descending);

        
        entities.Should()
            .HaveSameCount(_listEntities)
            .And
            .ContainInOrder(_listEntities.OrderByDescending(e => e.Property));
    }

    [Test]
    public async Task GetListPaginated()
    {
        const int pageSize = 6;


        var entities = await _repository.GetListAsync(1, pageSize);
        var entitiesLastPage = await _repository.GetListAsync(3, pageSize);


        entities.Meta.Total.Should().Be(entitiesLastPage.Meta.Total);
        entities.Meta.RecordsInDataset.Should().Be(pageSize);
        entities.Meta.CurrentPage.Should().Be(1);
        entities.Meta.PerPage.Should().Be(pageSize);
        entities.Meta.TotalPages.Should().Be(3);

        entitiesLastPage.Meta.RecordsInDataset.Should().Be(3);
        entitiesLastPage.Meta.CurrentPage.Should().Be(3);
        entitiesLastPage.Meta.PerPage.Should().Be(pageSize);
        entitiesLastPage.Meta.TotalPages.Should().Be(3);
    }

    [Test]
    public async Task GetListWithSelectReturnsAll()
    {
        var entities = await _repository.GetListWithSelectAsync<string>(e => e.Property);


        entities.Should().HaveCount(_listEntities.Count() + 1);
    }

    [Test]
    public async Task GetListWithSelectWhere()
    {
        const string propertyToLookFor = "b";
        
        
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property == propertyToLookFor
        });


        entities.Should().HaveCount(1);
        entities.ElementAt(0).Should().Be(propertyToLookFor);
    }

    [Test]
    public async Task GetListWithSelectOrderBy()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property.Length == 1
        }, x => x.Property);


        entities.Should()
            .HaveSameCount(_listEntities)
            .And
            .ContainInOrder(_listEntities.Select(e => e.Property).OrderBy(e => e));
    }

    [Test]
    public async Task GetListWithSelectOrderByDescending()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[]
        {
            x => x.Property.Length == 1
        }, x => x.Property, OrderBy.Descending);

        
        entities.Should()
            .HaveSameCount(_listEntities)
            .And
            .ContainInOrder(_listEntities.Select(e => e.Property).OrderByDescending(e => e));
    }

    [Test]
    public async Task GetListWithSelectPaginated()
    {
        const int pageSize = 6;


        var entities = await _repository.GetListWithSelectAsync(x => x.Property, 1, pageSize);
        var entitiesLastPage = await _repository.GetListWithSelectAsync(x => x.Property, 3, pageSize);

        
        entities.Meta.Total.Should().Be(entitiesLastPage.Meta.Total);
        entities.Meta.RecordsInDataset.Should().Be(pageSize);
        entities.Meta.CurrentPage.Should().Be(1);
        entities.Meta.PerPage.Should().Be(pageSize);
        entities.Meta.TotalPages.Should().Be(3);

        entitiesLastPage.Meta.RecordsInDataset.Should().Be(3);
        entitiesLastPage.Meta.CurrentPage.Should().Be(3);
        entitiesLastPage.Meta.PerPage.Should().Be(pageSize);
        entitiesLastPage.Meta.TotalPages.Should().Be(3);
    }

    private IEnumerable<TestEntity> GetTestList()
    {
        return new List<TestEntity>
        {
            GetTestEntity("a"),
            GetTestEntity("b"),
            GetTestEntity("c"),
            GetTestEntity("d"),
            GetTestEntity("e"),
            GetTestEntity("f"),
            GetTestEntity("g"),
            GetTestEntity("h"),
            GetTestEntity("i"),
            GetTestEntity("j"),
            GetTestEntity("k"),
            GetTestEntity("l"),
            GetTestEntity("m"),
            GetTestEntity("n")
        };
    }

    private TestEntity GetTestEntity(string property)
    {
        return new TestEntity
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Property = property
        };
    }
    #endregion

    #region Get
    [Test]
    public async Task GetValidEntityReturnsEntity()
    {
        var entity = await _repository.GetAsync(_entity.Id);

        
        entity.Should().BeSameAs(_entity);
    }

    [Test]
    [AutoData]
    public async Task DontGetNonExistantEntity(Guid nonExistantId)
    {
        var entity = await _repository.GetAsync(nonExistantId);

        
        entity.Should().BeNull();
    }
    #endregion

    #region Update
    [Test]
    [AutoData]
    public async Task UpdateUpdatesUpdated(string propertyValue)
    {
        var oldUpdated = _entity.Updated;
        var oldCreated = _entity.Created;
        var entity = new TestEntity
        {
            Id = _entity.Id,
            Property = propertyValue
        };

        
        var updatedEntity = await _repository.UpdateAsync(entity);


        updatedEntity.Property.Should().Be(propertyValue);
        updatedEntity.Updated.Should().NotBe(oldUpdated);
        updatedEntity.Created.Should().Be(oldCreated);
    }

    [Test]
    public Task UpdateThrowsExceptionIfNull()
    {
        return _repository.Awaiting(r => r.UpdateAsync(null))
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [AutoData]
    public async Task UpdateReadOnlyPropertyDoesNothing(string propertyValue, string readOnlyValue)
    {
        var entity = new TestEntity
        {
            Id = _entity.Id,
            Property = propertyValue,
            ReadOnlyProperty = readOnlyValue
        };


        var updatedEntity = await _repository.UpdateAsync(entity);


        updatedEntity.Property.Should().Be(propertyValue);
        updatedEntity.ReadOnlyProperty.Should().NotBe(readOnlyValue);
    }
    #endregion

    #region Delete
    [Test]
    public async Task DeleteDeletesEntity()
    {
        var expectedEntityCount = _context.Table.Count() - 1;
        var success = await _repository.DeleteAsync(_entity);
        await _unitOfWork.CommitAsync();

        
        var newlyDeletedEntity = await _repository.GetAsync(_entity.Id);


        success.Should().BeTrue();
        newlyDeletedEntity.Should().BeNull();
        _context.Table.Should().HaveCount(expectedEntityCount);
    }
    [Test]
    public async Task DeleteOnIdDeletesEntity()
    {
        var expectedEntityCount = _context.Table.Count() - 1;
        var success = await _repository.DeleteAsync(_entity.Id);
        await _unitOfWork.CommitAsync();

        
        var newlyDeletedEntity = await _repository.GetAsync(_entity.Id);


        success.Should().BeTrue();
        newlyDeletedEntity.Should().BeNull();
        _context.Table.Should().HaveCount(expectedEntityCount);
    }

    [Test]
    public Task DeleteThrowsExceptionIfArgumentNull()
    {
        return _repository
            .Awaiting(r => r.DeleteAsync(null))
            .Should()
            .ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [AutoData]
    public async Task DeleteWithInvalidIdReturnsFalse(Guid randomId)
    {
        var success = await _repository.DeleteAsync(randomId);


        success.Should().BeFalse();
    }

    [Test]
    public Task DeleteWithEmptyGuidThrowsException()
    {
        return _repository
            .Awaiting(r => r.DeleteAsync(Guid.Empty))
            .Should()
            .ThrowAsync<ArgumentException>();
    }
    #endregion
}
