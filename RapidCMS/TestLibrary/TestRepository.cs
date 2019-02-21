using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Interfaces;

namespace TestLibrary
{
    public abstract class TestRepository : BaseRepository<int, TestEntity>
    {
        private readonly Dictionary<int, List<TestEntity>> _data = new Dictionary<int, List<TestEntity>>();

        private List<TestEntity> GetData(int? parentId)
        {
            // TODO: this broke inserting documents

            if (!_data.ContainsKey(parentId ?? 0))
            {
                _data[parentId ?? 0] = new[]
                {
                    new TestEntity { Id = 1, Name = Guid.NewGuid().ToString(), Description = "Entity 1 Description", Number = 10 },
                    new TestEntity { Id = 2, Name = Guid.NewGuid().ToString(), Description = "Entity 2 Description", Number = 20 }
                }.ToList();
            }

            return _data[parentId ?? 0];
        }

        protected abstract string Name { get; }

        public override async Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId)
        {
            await Task.Delay(1);

            return GetData(parentId);
        }

        public override async Task<TestEntity> GetByIdAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            return GetData(parentId).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<TestEntity> InsertAsync(int id, int? parentId, TestEntity entity)
        {
            await Task.Delay(1);

            entity.Id = GetData(parentId).Any() ? GetData(parentId).Max(x => x.Id) + 1 : 1;

            GetData(parentId).Add(entity);

            return entity;
        }

        public override async Task UpdateAsync(int id, int? parentId, TestEntity entity)
        {
            await Task.Delay(1);

            var element = GetData(parentId).First(x => x.Id == id);

            element.Description = entity.Description;
            element.Name = entity.Name;
        }

        public override Task<TestEntity> NewAsync(int? parentId)
        {
            return Task.FromResult(new TestEntity
            {

            });
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            GetData(parentId).RemoveAll(x => x.Id == id);
        }
    }

    public class RepositoryA : TestRepository
    {
        protected override string Name => nameof(RepositoryA);
    }
    public class RepositoryB : TestRepository
    {
        protected override string Name => nameof(RepositoryB);
    }
    public class RepositoryC : TestRepository
    {
        protected override string Name => nameof(RepositoryC);
    }
    public class RepositoryD : TestRepository
    {
        protected override string Name => nameof(RepositoryD);
    }
    public class RepositoryE : TestRepository
    {
        protected override string Name => nameof(RepositoryE);
    }

    public class TestEntity : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
    }
}
