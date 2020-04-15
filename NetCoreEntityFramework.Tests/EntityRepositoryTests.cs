using AutoFixture.NUnit3;
using Microsoft.EntityFrameworkCore;
using Nodes.NetCore.EntityFramework.Enums;
using Nodes.NetCore.EntityFramework.Tests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestContext = Nodes.NetCore.EntityFramework.Tests.Mocks.TestContext;

namespace Nodes.NetCore.EntityFramework.Tests
{
    public class EntityRepositoryTests
    {
        private TestEntityRepository _repository;
        private TestContext _context;
        private TestEntity _entity;
        private TestEntity _deletedEntity;
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
                Deleted = false,
                Id = Guid.NewGuid(),
                Updated = now,
                Property = string.Empty
            };

            _deletedEntity = new TestEntity
            {
                Created = now.AddMinutes(-42),
                Deleted = true,
                DeletedAt = now,
                Id = Guid.NewGuid(),
                Updated = now.AddMinutes(-42),
                Property = "I'm deleted"
            };

            _context.Table.Add(_entity);
            _context.Table.Add(_deletedEntity);

            _listEntities = GetTestList();
            _context.Table.AddRange(_listEntities);

            _context.SaveChanges();

            _repository = new TestEntityRepository(_context);
        }

        #region Add
        [Test]
        public async Task AddAddsEntityAndSetsAttributes()
        {
            int startSize = await _context.Table.CountAsync();
            int expectedSize = startSize + 1;
            var entity = new TestEntity();

            using(_repository)
            {
                await _repository.Add(entity);
            }

            Assert.NotNull(entity.Id);
            Assert.AreNotEqual(default(DateTime), entity.Created);
            Assert.AreNotEqual(default(DateTime), entity.Updated);
            Assert.IsFalse(entity.Deleted);
            Assert.AreEqual(expectedSize, await _context.Table.CountAsync());
        }

        [Test]
        public async Task AddEntityWithIdKeepsId()
        {
            Guid id = Guid.NewGuid();
            var entity = new TestEntity
            {
                Id = id
            };

            using (_repository)
            {
                await _repository.Add(entity);
            }

            Assert.AreEqual(id, entity.Id);
        }

        [Test]
        public void AddThrowsExceptionIfEntityIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Add(null));
        }
        #endregion

        #region List
        [Test]
        public async Task GetListReturnsAllNotDeleted()
        {
            var entities = await _repository.GetList();

            Assert.AreEqual(_listEntities.Count() + 1, entities.Count());
        }

        [Test]
        public async Task GetListReturnsAll()
        {
            var entities = await _repository.GetList(null, null, OrderBy.Ascending, GetListMode.IncludeDeleted);

            Assert.AreEqual(_listEntities.Count() + 2, entities.Count());
        }

        [Test]
        public async Task GetListReturnsAllDeleted()
        {
            var entities = await _repository.GetList(null, null, OrderBy.Ascending, GetListMode.OnlyDeleted);

            Assert.AreEqual(1, entities.Count());
        }

        [Test]
        public async Task GetListWhere()
        {
            var entities = await _repository.GetList(x => x.Property == "b");

            Assert.AreEqual(1, entities.Count());
            Assert.AreSame(_listEntities.First(x => x.Property == "b"), entities.ElementAt(0));
        }

        [Test]
        public async Task GetListOrderBy()
        {
            var entities = await _repository.GetList(x => x.Property.Length == 1, x => x.Property);

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
            var entities = await _repository.GetList(x => x.Property.Length == 1, x => x.Property, OrderBy.Descending);

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

            var entities = await _repository.GetList(1, pageSize);
            var entitiesLastPage = await _repository.GetList(3, pageSize);

            Assert.AreEqual(pageSize, entities.Count());
            Assert.AreEqual(3, entitiesLastPage.Count());
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
            var entity = await _repository.Get((Guid)_entity.Id);

            Assert.AreSame(_entity, entity);
        }

        [Test]
        public async Task DontGetDeletedEntityWithoutFlag()
        {
            var entity = await _repository.Get((Guid)_deletedEntity.Id);

            Assert.IsNull(entity);
        }

        [Test]
        public async Task GetDeletedEntityWithFlag()
        {
            var entity = await _repository.Get((Guid)_deletedEntity.Id, true);

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

            using(_repository)
            {
                await _repository.Update(_entity);
            }

            var entity = await _repository.Get((Guid)_entity.Id);

            Assert.AreEqual(propertyValue, entity.Property);
            Assert.AreNotEqual(oldUpdated, entity.Updated);
            Assert.AreEqual(oldCreated, entity.Created);
        }

        [Test]
        public void UpdateThrowsExceptionIfNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Update(null));
        }
        #endregion

        #region Delete
        [Test]
        public async Task DeleteSoftDeletesAndSetsDeletedAt()
        {
            bool success;
            using(_repository)
            {
                success = await _repository.Delete(_entity);
            }

            var newlyDeletedEntity = await _repository.Get((Guid)_entity.Id, true);
            Assert.IsTrue(success);
            Assert.IsTrue(newlyDeletedEntity.Deleted);
            Assert.NotNull(newlyDeletedEntity.DeletedAt);
        }

        [Test]
        public void DeleteThrowsExceptionIfArgumentNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Delete(null));
        }

        [Test]
        public async Task DeleteWithValidIdDeletesAndSetsDeletedAt()
        {
            bool success;
            Guid id = (Guid)_entity.Id;
            using (_repository)
            {
                success = await _repository.Delete(id);
            }

            var newlyDeletedEntity = await _repository.Get(id, true);
            Assert.IsTrue(success);
            Assert.IsTrue(newlyDeletedEntity.Deleted);
            Assert.NotNull(newlyDeletedEntity.DeletedAt);
        }

        [Test]
        [AutoData]
        public async Task DeleteWithInvalidIdReturnsFalse(Guid randomId)
        {
            bool success;

            using(_repository)
            {
                success = await _repository.Delete(randomId);
            }

            Assert.IsFalse(success);
        }

        [Test]
        public void DeleteWithEmptyGuidThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _repository.Delete(Guid.Empty));
        }
        #endregion

        #region Restore
        [Test]
        public async Task RestoreSetsDeletedFalse()
        {
            bool success;

            using(_repository)
            {
                success = await _repository.Restore(_deletedEntity);
            }

            var restoredEntity = await _repository.Get((Guid)_deletedEntity.Id);
            Assert.IsTrue(success);
            Assert.IsFalse(restoredEntity?.Deleted);
        }

        [Test]
        public void RestoreThrowsExceptionWhenEntityNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Restore(null));
        }

        [Test]
        public async Task RestoreOnIdSetsDeletedFalse()
        {
            bool success;
            Guid id = (Guid)_deletedEntity.Id;

            using (_repository)
            {
                success = await _repository.Restore(id);
            }

            var restoredEntity = await _repository.Get(id);
            Assert.IsTrue(success);
            Assert.IsFalse(restoredEntity?.Deleted);
        }

        [Test]
        [AutoData]
        public async Task RestoreOnInvalidIdReturnsFalse(Guid randomId)
        {
            bool success;

            using(_repository)
            {
                success = await _repository.Restore(randomId);
            }

            Assert.IsFalse(success);
        }

        [Test]
        public void RestoreOnEmptyGuidThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _repository.Restore(Guid.Empty));
        }
        #endregion
    }
}