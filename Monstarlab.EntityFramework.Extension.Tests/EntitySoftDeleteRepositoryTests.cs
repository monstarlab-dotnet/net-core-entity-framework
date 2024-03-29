﻿namespace Monstarlab.EntityFramework.Extension.Tests;

public class EntitySoftDeleteRepositoryTests
{
    private TestSoftDeleteEntityRepository _repository;
    private TestContext _context;
    private TestSoftDeleteEntity _entity;
    private TestSoftDeleteEntity _deletedEntity;
    private IEnumerable<TestSoftDeleteEntity> _listEntities;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _context = new TestContext(options);

        _repository = new TestSoftDeleteEntityRepository(_context);

        DateTime now = DateTime.UtcNow;

        _entity = new TestSoftDeleteEntity
        {
            Created = now,
            Deleted = false,
            Id = Guid.NewGuid(),
            Updated = now,
            Property = string.Empty
        };

        _deletedEntity = new TestSoftDeleteEntity
        {
            Created = now.AddMinutes(-42),
            Deleted = true,
            DeletedAt = now,
            Id = Guid.NewGuid(),
            Updated = now.AddMinutes(-42),
            Property = "I'm deleted"
        };

        _context.SoftDeleteTable.Add(_entity);
        _context.SoftDeleteTable.Add(_deletedEntity);

        _listEntities = GetTestList();
        _context.SoftDeleteTable.AddRange(_listEntities);

        _context.SaveChanges();

        _repository = new TestSoftDeleteEntityRepository(_context);
    }

    #region Add
    [Test]
    public async Task AddAddsEntityAndSetsAttributes()
    {
        int startSize = await _context.SoftDeleteTable.CountAsync();
        int expectedSize = startSize + 1;
        var entity = new TestSoftDeleteEntity();

        await _repository.AddAsync(entity);

        Assert.AreNotEqual(Guid.Empty, entity.Id);
        Assert.AreNotEqual(default(DateTime), entity.Created);
        Assert.AreNotEqual(default(DateTime), entity.Updated);
        Assert.IsFalse(entity.Deleted);
        Assert.AreEqual(expectedSize, await _context.SoftDeleteTable.CountAsync());
    }

    [Test]
    public async Task AddEntityWithIdKeepsId()
    {
        Guid id = Guid.NewGuid();
        var entity = new TestSoftDeleteEntity
        {
            Id = id
        };

        await _repository.AddAsync(entity);

        Assert.AreEqual(id, entity.Id);
    }

    [Test]
    public void AddThrowsExceptionIfEntityIsNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null));
    }
    #endregion

    #region List
    [Test]
    public async Task GetListReturnsAllNotDeleted()
    {
        var entities = await _repository.GetListAsync();

        Assert.AreEqual(_listEntities.Count() + 1, entities.Count());
    }

    [Test]
    public async Task GetListReturnsAll()
    {
        var entities = await _repository.GetListAsync(null, null, OrderBy.Ascending, GetListMode.IncludeDeleted);

        Assert.AreEqual(_listEntities.Count() + 2, entities.Count());
    }

    [Test]
    public async Task GetListReturnsAllDeleted()
    {
        var entities = await _repository.GetListAsync(null, null, OrderBy.Ascending, GetListMode.OnlyDeleted);

        Assert.AreEqual(1, entities.Count());
    }

    [Test]
    public async Task GetListWhere()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property == "b" });

        Assert.AreEqual(1, entities.Count());
        Assert.AreSame(_listEntities.First(x => x.Property == "b"), entities.ElementAt(0));
    }

    [Test]
    public async Task GetListOrderBy()
    {
        var entities = await _repository.GetListAsync(new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property);

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
        var entities = await _repository.GetListAsync(new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property, OrderBy.Descending);

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

    private IEnumerable<TestSoftDeleteEntity> GetTestList()
    {
        return new List<TestSoftDeleteEntity>
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

    private TestSoftDeleteEntity GetTestEntity(string property)
    {
        return new TestSoftDeleteEntity
        {
            Id = Guid.NewGuid(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Property = property
        };
    }
    [Test]
    public async Task GetListWithReturnsReturnsAllNotDeleted()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property);

        Assert.AreEqual(_listEntities.Count() + 1, entities.Count());
    }

    [Test]
    public async Task GetListWithSelectReturnsAll()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, null, null, OrderBy.Ascending, GetListMode.IncludeDeleted);

        Assert.AreEqual(_listEntities.Count() + 2, entities.Count());
    }

    [Test]
    public async Task GetListWithSelectReturnsAllDeleted()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, null, null, OrderBy.Ascending, GetListMode.OnlyDeleted);

        Assert.AreEqual(1, entities.Count());
    }

    [Test]
    public async Task GetListWithSelectWhere()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property == "b" });

        Assert.AreEqual(1, entities.Count());
        Assert.AreEqual("b", entities.ElementAt(0));
    }

    [Test]
    public async Task GetListWithSelectOrderBy()
    {
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property);

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
        var entities = await _repository.GetListWithSelectAsync(x => x.Property, new Expression<Func<TestSoftDeleteEntity, bool>>[] { x => x.Property.Length == 1 }, x => x.Property, OrderBy.Descending);

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
    #endregion

    #region Get
    [Test]
    public async Task GetValidEntityReturnsEntity()
    {
        var entity = await _repository.GetAsync(_entity.Id);

        Assert.AreSame(_entity, entity);
    }

    [Test]
    public async Task DontGetDeletedEntityWithoutFlag()
    {
        var entity = await _repository.GetAsync(_deletedEntity.Id);

        Assert.IsNull(entity);
    }

    [Test]
    public async Task GetDeletedEntityWithFlag()
    {
        var entity = await _repository.GetAsync(_deletedEntity.Id, GetListMode.IncludeDeleted);

        Assert.AreSame(_deletedEntity, entity);
    }
    #endregion

    #region Update
    [Test]
    [AutoData]
    public async Task UpdateUpdatesUpdated(string propertyValue)
    {
        DateTime oldUpdated = _entity.Updated;
        DateTime oldCreated = _entity.Created;
        _entity.Property = propertyValue;

        var entity = await _repository.UpdateAsync(_entity);

        Assert.AreEqual(propertyValue, entity.Property);
        Assert.AreNotEqual(oldUpdated, entity.Updated);
        Assert.AreEqual(oldCreated, entity.Created);
    }

    [Test]
    public void UpdateThrowsExceptionIfNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(null));
    }
    #endregion

    #region Delete
    [Test]
    public async Task DeleteSoftDeletesAndSetsDeletedAt()
    {
        bool success = await _repository.DeleteAsync(_entity);

        var newlyDeletedEntity = await _repository.GetAsync(_entity.Id, GetListMode.IncludeDeleted);
        Assert.IsTrue(success);
        Assert.IsTrue(newlyDeletedEntity.Deleted);
        Assert.NotNull(newlyDeletedEntity.DeletedAt);
    }

    [Test]
    public void DeleteThrowsExceptionIfArgumentNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(null));
    }

    [Test]
    public async Task DeleteWithValidIdDeletesAndSetsDeletedAt()
    {
        Guid id = _entity.Id;
        bool success = await _repository.DeleteAsync(id);

        var newlyDeletedEntity = await _repository.GetAsync(id, GetListMode.IncludeDeleted);
        Assert.IsTrue(success);
        Assert.IsTrue(newlyDeletedEntity.Deleted);
        Assert.NotNull(newlyDeletedEntity.DeletedAt);
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

    #region Restore
    [Test]
    public async Task RestoreSetsDeletedFalse()
    {
        var restoredEntity = await _repository.RestoreAsync(_deletedEntity);

        Assert.IsFalse(restoredEntity.Deleted);
        Assert.IsNull(restoredEntity.DeletedAt);
    }

    [Test]
    public void RestoreThrowsExceptionWhenEntityNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.RestoreAsync(null));
    }

    [Test]
    public async Task RestoreOnIdSetsDeletedFalse()
    {
        var restoredEntity = await _repository.RestoreAsync(_deletedEntity.Id);

        Assert.IsFalse(restoredEntity.Deleted);
        Assert.IsNull(restoredEntity.DeletedAt);
    }

    [Test]
    [AutoData]
    public async Task RestoreOnInvalidIdReturnsFalse(Guid randomId)
    {
        var restoredEntity = await _repository.RestoreAsync(randomId);

        Assert.IsNull(restoredEntity);
    }

    [Test]
    public void RestoreOnEmptyGuidThrowsException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _repository.RestoreAsync(Guid.Empty));
    }
    #endregion
}
