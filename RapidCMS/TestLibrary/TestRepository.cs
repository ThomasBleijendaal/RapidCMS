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
        protected abstract string Name { get; }

        public override async Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId)
        {
            await Task.Delay(1);

            return new[]
            {
                new TestEntity { Id = (parentId ?? 0) * 10 + 1, Name = $"{Name} Entity 1" },
                new TestEntity { Id = (parentId ?? 0) * 10 + 2, Name = $"{Name} Entity 2" }
            };
        }

        public override async Task<TestEntity> GetByIdAsync(int id, int? parentId)
        {
            var data = await GetAllAsync(parentId);
            return data.FirstOrDefault(x => x.Id == id);
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
    }
}
