using AutoFixture.NUnit3;
using Microsoft.EntityFrameworkCore;
using Nodes.NetCore.EntityFramework.Tests.Mocks;
using NUnit.Framework;
using System;
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
    }
}