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
        private List<TestEntity> _data = null;

        private List<TestEntity> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new[]
                    {
                        new TestEntity { Id = 1, Name = Guid.NewGuid().ToString(), Description = "Entity 1 Description" },
                        new TestEntity { Id = 2, Name = Guid.NewGuid().ToString(), Description = "Entity 2 Description" }
                    }.ToList();
                }

                return _data;
            }
        }

        protected abstract string Name { get; }

        public override async Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId)
        {
            await Task.Delay(1);

            return Data;
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
        public string Description { get; set; }
    }
}
