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

            _context.Table.Add(_entity);

            _context.SaveChanges();

            _repository = new TestEntityRepository(_context);
        }

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
    }
}