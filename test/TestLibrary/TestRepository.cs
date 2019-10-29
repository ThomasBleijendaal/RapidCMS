using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

namespace TestLibrary
{
    public abstract class TestRepository : BaseStructRepository<int, int, TestEntity>
    {
        private readonly Dictionary<int, List<TestEntity>> _data = new Dictionary<int, List<TestEntity>>();

        private List<TestEntity> GetData(int? parentId)
        {
            if (!_data.ContainsKey(parentId ?? 0))
            {
                _data[parentId ?? 0] = new[]
                {
                    new TestEntity { _Id = 1, Name = Guid.NewGuid().ToString(), Description = "Entity 1 Description", Number = 10 },
                    new TestEntity { _Id = 2, Name = Guid.NewGuid().ToString(), Description = "Entity 2 Description", Number = 20 }
                }
                .ToList();
            }

            return _data[parentId ?? 0];
        }

        protected abstract string Name { get; }

        public override async Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId, IQuery<TestEntity> query)
        {
            await Task.Delay(1);

            return GetData(parentId);
        }

        public override async Task<TestEntity> GetByIdAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            return GetData(parentId).FirstOrDefault(x => x._Id == id);
        }

        public override async Task<TestEntity> InsertAsync(int? parentId, TestEntity entity, IRelationContainer? relations)
        {
            await Task.Delay(1);

            entity._Id = GetData(parentId).Any() ? GetData(parentId).Max(x => x._Id) + 1 : 1;

            GetData(parentId).Add(entity);

            return entity;
        }

        public override async Task UpdateAsync(int id, int? parentId, TestEntity entity, IRelationContainer? relations)
        {
            await Task.Delay(1);

            var element = GetData(parentId).First(x => x._Id == id);

            element.Description = entity.Description;
            element.Name = entity.Name;
        }

        public override Task<TestEntity> NewAsync(int? parentId, Type? variantType)
        {
            return Task.FromResult(new TestEntity
            {

            });
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            await Task.Delay(1);

            GetData(parentId).RemoveAll(x => x._Id == id);
        }

        public override int ParseKey(string id)
        {
            return int.TryParse(id, out var intId) ? intId : 0;
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var intParentId)
                    ? intParentId
                    : default(int?);
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
    public class RepositoryF : TestRepository
    {
        protected override string Name => nameof(RepositoryF);
    }
    public class VariantRepository : BaseStructRepository<int, int, TestEntity>
    {
        private readonly List<TestEntity> _data = new List<TestEntity>
        {
            new TestEntityVariantA { _Id = 1, ParentId = null, Description = "Variant A", Name = "A", Number = 1, Title = "This is the title" },
            new TestEntityVariantB { _Id = 2, ParentId = null, Description = "Variant B", Name = "B", Number = 2, Image = "This is the image" },
            new TestEntityVariantC { _Id = 3, ParentId = null, Description = "Variant C", Name = "C", Number = 3, Quote = "This is the quote" },
        };

        public override Task DeleteAsync(int id, int? parentId)
        {
            _data.RemoveAll(x => x._Id == id && x.ParentId == parentId);

            return Task.CompletedTask;
        }

        public override Task<IEnumerable<TestEntity>> GetAllAsync(int? parentId, IQuery<TestEntity> query)
        {
            return Task.FromResult(_data.Where(x => x.ParentId == parentId));
        }

        public override Task<TestEntity> GetByIdAsync(int id, int? parentId)
        {
            return Task.FromResult(_data.FirstOrDefault(x => x._Id == id && x.ParentId == parentId));
        }

        public override async Task<TestEntity> InsertAsync(int? parentId, TestEntity entity, IRelationContainer? relations)
        {
            await Task.Delay(1);

            entity._Id = _data.Any() ? _data.Max(x => x._Id) + 1 : 1;
            entity.ParentId = parentId;

            _data.Add(entity);

            return entity;
        }

        public override Task<TestEntity> NewAsync(int? parentId, Type? variantType)
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
                return Task.FromResult(new TestEntity());
            }
        }

        public override async Task UpdateAsync(int id, int? parentId, TestEntity entity, IRelationContainer? relations)
        {
            await Task.Delay(1);

            var element = _data.First(x => x._Id == id);

            element.ParentId = entity.ParentId;
            element.Description = entity.Description;
            element.Name = entity.Name;
            element.Number = entity.Number;

            switch (entity)
            {
                case TestEntityVariantA a:
                    (element as TestEntityVariantA).Title = a.Title;
                    break;
                case TestEntityVariantB b:
                    (element as TestEntityVariantB).Image = b.Image;
                    break;
                case TestEntityVariantC c:
                    (element as TestEntityVariantC).Quote = c.Quote;
                    break;
            }
        }

        public override int ParseKey(string id)
        {
            return int.TryParse(id, out var intId) ? intId : 0;
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var intParentId)
                    ? intParentId
                    : default(int?);
        }
    }
}
