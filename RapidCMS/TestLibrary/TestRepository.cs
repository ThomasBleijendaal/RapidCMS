using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Data;
using RapidCMS.Common.Interfaces;

namespace TestLibrary
{
    public class TestRepository : BaseRepository<int, TestEntity>
    {
        public override IEnumerable<TestEntity> GetAll()
        {
            return new[]
            {
                new TestEntity { Id = 1, Name = "Entity 1" },
                new TestEntity { Id = 2, Name = "Entity 2"}
            };
        }

        public override TestEntity GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }
    }

    public class TestEntity : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
