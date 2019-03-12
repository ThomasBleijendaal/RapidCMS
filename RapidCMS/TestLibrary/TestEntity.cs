using RapidCMS.Common.Interfaces;

namespace TestLibrary
{
    public class TestEntity : IEntity
    {
        public string Id
        {
            get
            {
                return _Id.ToString();
            }
            set
            {
                _Id = int.TryParse(value, out var id) ? id : 0;
            }
        }

        public int? ParentId { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        public int _Id { get; set; }
#pragma warning restore IDE1006 // Naming Styles

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
