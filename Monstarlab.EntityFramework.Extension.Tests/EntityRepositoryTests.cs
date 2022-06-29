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

        DateTime now = DateTime.UtcNow;

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

        Assert.AreNotEqual(Guid.Empty, addedEntity.Id);
        Assert.AreNotEqual(default(DateTime), addedEntity.Created);
        Assert.AreNotEqual(default(DateTime), addedEntity.Updated);
        Assert.AreEqual(expectedSize, await _context.Table.CountAsync());
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

        Assert.AreEqual(idToCreate, addedEntity.Id);
    }

    [Test]
    public void AddThrowsExceptionIfEntityIsNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null));
    }
    #endregion

    #region List
    [Test]
    public async Task GetListReturnsAll()
    {
        var entities = await _repository.GetListAsync(null, null, OrderBy.Ascending);

        Assert.AreEqual(_listEntities.Count() + 1, entities.Count());
    }

    [Test]
    public async Task GetListWhere()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[] { x => x.Property == "b" });

        Assert.AreEqual(1, entities.Count());
        Assert.AreSame(_listEntities.First(x => x.Property == "b"), entities.ElementAt(0));
    }

    [Test]
    public async Task GetListOrderBy()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property);

        Assert.AreEqual(_listEntities.Count(), entities.Count());
        Assert.AreSame(_listEntities.First(x => x.Property == "a"), entities.ElementAt(0));
        Assert.AreSame(_listEntities.First(x => x.Property == "b"), entities.ElementAt(1));
        Assert.AreSame(_listEntities.First(x => x.Property == "c"), entities.ElementAt(2));
        Assert.AreSame(_listEntities.First(x => x.Property == "d"), entities.ElementAt(3));
        Assert.AreSame(_listEntities.First(x => x.Property == "e"), entities.ElementAt(4));
        Assert.AreSame(_listEntities.First(x => x.Property == "f"), entities.ElementAt(5));
        Assert.AreSame(_listEntities.First(x => x.Property == "g"), entities.ElementAt(6));
        Assert.AreSame(_listEntities.First(x => x.Property == "h"), entities.ElementAt(7));
        Assert.AreSame(_listEntities.First(x => x.Property == "i"), entities.ElementAt(8));
        Assert.AreSame(_listEntities.First(x => x.Property == "j"), entities.ElementAt(9));
        Assert.AreSame(_listEntities.First(x => x.Property == "k"), entities.ElementAt(10));
        Assert.AreSame(_listEntities.First(x => x.Property == "l"), entities.ElementAt(11));
        Assert.AreSame(_listEntities.First(x => x.Property == "m"), entities.ElementAt(12));
        Assert.AreSame(_listEntities.First(x => x.Property == "n"), entities.ElementAt(13));
    }

    [Test]
    public async Task GetListOrderByDescending()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property, OrderBy.Descending);

        Assert.AreEqual(_listEntities.Count(), entities.Count());
        Assert.AreSame(_listEntities.First(x => x.Property == "n"), entities.ElementAt(0));
        Assert.AreSame(_listEntities.First(x => x.Property == "m"), entities.ElementAt(1));
        Assert.AreSame(_listEntities.First(x => x.Property == "l"), entities.ElementAt(2));
        Assert.AreSame(_listEntities.First(x => x.Property == "k"), entities.ElementAt(3));
        Assert.AreSame(_listEntities.First(x => x.Property == "j"), entities.ElementAt(4));
        Assert.AreSame(_listEntities.First(x => x.Property == "i"), entities.ElementAt(5));
        Assert.AreSame(_listEntities.First(x => x.Property == "h"), entities.ElementAt(6));
        Assert.AreSame(_listEntities.First(x => x.Property == "g"), entities.ElementAt(7));
        Assert.AreSame(_listEntities.First(x => x.Property == "f"), entities.ElementAt(8));
        Assert.AreSame(_listEntities.First(x => x.Property == "e"), entities.ElementAt(9));
        Assert.AreSame(_listEntities.First(x => x.Property == "d"), entities.ElementAt(10));
        Assert.AreSame(_listEntities.First(x => x.Property == "c"), entities.ElementAt(11));
        Assert.AreSame(_listEntities.First(x => x.Property == "b"), entities.ElementAt(12));
        Assert.AreSame(_listEntities.First(x => x.Property == "a"), entities.ElementAt(13));
    }

    [Test]
    public async Task GetListPaginated()
    {
        const int pageSize = 6;


        var entities = await _repository.GetListAsync(1, pageSize);
        var entitiesLastPage = await _repository.GetListAsync(3, pageSize);


        Assert.AreEqual(entities.Meta.Total, entitiesLastPage.Meta.Total);
        Assert.AreEqual(pageSize, entities.Meta.RecordsInDataset);
        Assert.AreEqual(1, entities.Meta.CurrentPage);
        Assert.AreEqual(pageSize, entities.Meta.PerPage);
        Assert.AreEqual(3, entities.Meta.TotalPages);

        Assert.AreEqual(3, entitiesLastPage.Meta.RecordsInDataset);
        Assert.AreEqual(3, entitiesLastPage.Meta.CurrentPage);
        Assert.AreEqual(pageSize, entitiesLastPage.Meta.PerPage);
        Assert.AreEqual(3, entitiesLastPage.Meta.TotalPages);
    }

    [Test]
    public async Task GetListWithSelectReturnsAll()
    {
        var entities = await _repository.GetListWithSelectAsync<string>(e => e.Property);

        Assert.AreEqual(_listEntities.Count() + 1, entities.Count());
    }

    [Test]
    public async Task GetListWithSelectWhere()
    {
        const string propertyToLookFor = "b";
        IEnumerable<string> entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[] { x => x.Property == propertyToLookFor });

        Assert.AreEqual(1, entities.Count());
        Assert.AreEqual(propertyToLookFor, entities.ElementAt(0));
    }

    [Test]
    public async Task GetListWithSelectOrderBy()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property);

        Assert.AreEqual(_listEntities.Count(), entities.Count());
        Assert.AreEqual("a", entities.ElementAt(0));
        Assert.AreEqual("b", entities.ElementAt(1));
        Assert.AreEqual("c", entities.ElementAt(2));
        Assert.AreEqual("d", entities.ElementAt(3));
        Assert.AreEqual("e", entities.ElementAt(4));
        Assert.AreEqual("f", entities.ElementAt(5));
        Assert.AreEqual("g", entities.ElementAt(6));
        Assert.AreEqual("h", entities.ElementAt(7));
        Assert.AreEqual("i", entities.ElementAt(8));
        Assert.AreEqual("j", entities.ElementAt(9));
        Assert.AreEqual("k", entities.ElementAt(10));
        Assert.AreEqual("l", entities.ElementAt(11));
        Assert.AreEqual("m", entities.ElementAt(12));
        Assert.AreEqual("n", entities.ElementAt(13));
    }

    [Test]
    public async Task GetListWithSelectOrderByDescending()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property, OrderBy.Descending);

        Assert.AreEqual(_listEntities.Count(), entities.Count());
        Assert.AreEqual("n", entities.ElementAt(0));
        Assert.AreEqual("m", entities.ElementAt(1));
        Assert.AreEqual("l", entities.ElementAt(2));
        Assert.AreEqual("k", entities.ElementAt(3));
        Assert.AreEqual("j", entities.ElementAt(4));
        Assert.AreEqual("i", entities.ElementAt(5));
        Assert.AreEqual("h", entities.ElementAt(6));
        Assert.AreEqual("g", entities.ElementAt(7));
        Assert.AreEqual("f", entities.ElementAt(8));
        Assert.AreEqual("e", entities.ElementAt(9));
        Assert.AreEqual("d", entities.ElementAt(10));
        Assert.AreEqual("c", entities.ElementAt(11));
        Assert.AreEqual("b", entities.ElementAt(12));
        Assert.AreEqual("a", entities.ElementAt(13));
    }

    [Test]
    public async Task GetListWithSelectPaginated()
    {
        const int pageSize = 6;


        var entities = await _repository.GetListWithSelectAsync(x => x.Property, 1, pageSize);
        var entitiesLastPage = await _repository.GetListWithSelectAsync(x => x.Property, 3, pageSize);


        Assert.AreEqual(entities.Meta.Total, entitiesLastPage.Meta.Total);
        Assert.AreEqual(pageSize, entities.Meta.RecordsInDataset);
        Assert.AreEqual(1, entities.Meta.CurrentPage);
        Assert.AreEqual(pageSize, entities.Meta.PerPage);
        Assert.AreEqual(3, entities.Meta.TotalPages);

        Assert.AreEqual(3, entitiesLastPage.Meta.RecordsInDataset);
        Assert.AreEqual(3, entitiesLastPage.Meta.CurrentPage);
        Assert.AreEqual(pageSize, entitiesLastPage.Meta.PerPage);
        Assert.AreEqual(3, entitiesLastPage.Meta.TotalPages);
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

        Assert.AreSame(_entity, entity);
    }

    [Test]
    [AutoData]
    public async Task DontGetNonExistantEntity(Guid nonExistantId)
    {
        var entity = await _repository.GetAsync(nonExistantId);

        Assert.IsNull(entity);
    }
    #endregion

    #region Update
    [Test]
    [AutoData]
    public async Task UpdateUpdatesUpdated(string propertyValue)
    {
        DateTime oldUpdated = _entity.Updated;
        DateTime oldCreated = _entity.Created;
        var entity = new TestEntity
        {
            Id = _entity.Id,
            Property = propertyValue
        };

        var updatedEntity = await _repository.UpdateAsync(entity);

        Assert.AreEqual(propertyValue, updatedEntity.Property);
        Assert.AreNotEqual(oldUpdated, updatedEntity.Updated);
        Assert.AreEqual(oldCreated, updatedEntity.Created);
    }

    [Test]
    public void UpdateThrowsExceptionIfNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(null));
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

        Assert.AreEqual(propertyValue, updatedEntity.Property);
        Assert.AreNotEqual(readOnlyValue, updatedEntity.ReadOnlyProperty);
    }
    #endregion

    #region Delete
    [Test]
    public async Task DeleteDeletesEntity()
    {
        var expectedEntityCount = _context.Table.Count() - 1;
        bool success = await _repository.DeleteAsync(_entity);
        await _unitOfWork.CommitAsync();

        var newlyDeletedEntity = await _repository.GetAsync(_entity.Id);
        Assert.IsTrue(success);
        Assert.IsNull(newlyDeletedEntity);
        Assert.AreEqual(expectedEntityCount, _context.Table.Count());
    }
    [Test]
    public async Task DeleteOnIdDeletesEntity()
    {
        var expectedEntityCount = _context.Table.Count() - 1;
        bool success = await _repository.DeleteAsync(_entity.Id);
        await _unitOfWork.CommitAsync();

        var newlyDeletedEntity = await _repository.GetAsync(_entity.Id);
        Assert.IsTrue(success);
        Assert.IsNull(newlyDeletedEntity);
        Assert.AreEqual(expectedEntityCount, _context.Table.Count());
    }

    [Test]
    public void DeleteThrowsExceptionIfArgumentNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(null));
    }

    [Test]
    [AutoData]
    public async Task DeleteWithInvalidIdReturnsFalse(Guid randomId)
    {
        bool success = await _repository.DeleteAsync(randomId);

        Assert.IsFalse(success);
    }

    [Test]
    public void DeleteWithEmptyGuidThrowsException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(Guid.Empty));
    }
    #endregion
}
