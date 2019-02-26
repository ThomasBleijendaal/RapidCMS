using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

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
                }
                .ToList();
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

        public override Task<TestEntity> NewAsync(int? parentId, Type variantType)
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
    public class VariantRepository : BaseRepository<int, TestEntity>
    {
        private readonly List<TestEntity> _data = new List<TestEntity>
        {
            new TestEntityVariantA { Id = 1, Description = "Variant A", Name = "A", Number = 1, Title = "This is the title" },
            new TestEntityVariantB { Id = 2, Description = "Variant B", Name = "B", Number = 2, Image = "This is the image" },
            new TestEntityVariantC { Id = 3, Description = "Variant C", Name = "C", Number = 3, Quote = "This is the quote" },
        };

        public override Task DeleteAsync(int id, int? parentId)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId)
        {
            return Task.FromResult(_data.AsEnumerable());
        }

        public override Task<TestEntity> GetByIdAsync(int id, int? parentId)
        {
            return Task.FromResult(_data.FirstOrDefault(x => x.Id == id));
        }

        public override Task<TestEntity> InsertAsync(int id, int? parentId, TestEntity entity)
        {
            throw new NotImplementedException();
        }

        public override Task<TestEntity> NewAsync(int? parentId, Type variantType)
        {
            if (variantType == typeof(TestEntityVariantA))
            {
                return Task.FromResult(new TestEntityVariantA() as TestEntity);
            }
            else if (variantType == typeof(TestEntityVariantB))
            {
                return Task.FromResult(new TestEntityVariantB() as TestEntity);
            }
            else if (variantType == typeof(TestEntityVariantC))
            {
                return Task.FromResult(new TestEntityVariantC() as TestEntity);
            }
            else
            {
                return Task.FromResult(default(TestEntity));
            }
        }

        public override Task UpdateAsync(int id, int? parentId, TestEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
