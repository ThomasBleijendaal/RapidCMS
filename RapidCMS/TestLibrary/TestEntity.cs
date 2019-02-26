using RapidCMS.Common.Interfaces;

namespace TestLibrary
{
    public class TestEntity : IEntity
    {
        public int? ParentId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
    }

    public class TestEntityVariantA : TestEntity
    {
        public string Title { get; set; }
    }

    public class TestEntityVariantB : TestEntity
    {
        public string Image { get; set; }
    }

    public class TestEntityVariantC : TestEntity
    {
        public string Quote { get; set; }
    }
}
