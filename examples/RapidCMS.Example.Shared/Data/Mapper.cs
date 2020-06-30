using RapidCMS.Repositories;

namespace RapidCMS.Example.Shared.Data
{
    public class Mapper : IConverter<MappedEntity, DatabaseEntity>
    {
        public MappedEntity Convert(DatabaseEntity obj)
        {
            return new MappedEntity
            {
                Description = obj.Description,
                Id = obj.Id,
                Name = obj.Name
            };
        }

        public DatabaseEntity Convert(MappedEntity obj)
        {
            return new DatabaseEntity
            {
                Description = obj.Description,
                Id = obj.Id,
                Name = obj.Name
            };
        }
    }
}
