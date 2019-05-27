using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Common.Data;
using TestLibrary.Data;
using TestLibrary.Entities;

namespace TestLibrary.Repositories
{
    public class CountryRepository : BaseStructRepository<int, int, CountryEntity>
    {
        private readonly TestDbContext _dbContext;

        public CountryRepository(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            var entry = new CountryEntity { _Id = id };
            _dbContext.Countries.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<CountryEntity>> GetAllAsync(int? parentId)
        {
            return await _dbContext.Countries.AsNoTracking().ToListAsync();
        }

        public override async Task<CountryEntity> GetByIdAsync(int id, int? parentId)
        {
            return await _dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(x => x._Id == id);
        }

        public override async Task<CountryEntity> InsertAsync(int? parentId, CountryEntity entity, IEnumerable<IRelation> relations)
        {
            var entry = _dbContext.Countries.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override Task<CountryEntity> NewAsync(int? parentId, Type variantType = null)
        {
            return Task.FromResult(new CountryEntity());
        }

        public override int ParseKey(string id)
        {
            return int.Parse(id);
        }

        public override int? ParseParentKey(string parentId)
        {
            return int.TryParse(parentId, out var id) ? id : default(int?);
        }

        public override async Task UpdateAsync(int id, int? parentId, CountryEntity entity, IEnumerable<IRelation> relations)
        {
            var dbEntity = await _dbContext.Countries.FirstOrDefaultAsync(x => x._Id == id);

            dbEntity.Name = entity.Name;

            _dbContext.Countries.Update(dbEntity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class PersonRepository : BaseStructRepository<int, int, PersonEntity>
    {
        private readonly TestDbContext _dbContext;

        public PersonRepository(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            var entry = new CountryEntity { _Id = id };
            _dbContext.Countries.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<PersonEntity>> GetAllAsync(int? parentId)
        {
            return await _dbContext.Persons.Include(x => x.Countries).AsNoTracking().ToListAsync();
        }

        public override async Task<PersonEntity> GetByIdAsync(int id, int? parentId)
        {
            return await _dbContext.Persons.Include(x => x.Countries).AsNoTracking().FirstOrDefaultAsync(x => x._Id == id);
        }

        public override async Task<PersonEntity> InsertAsync(int? parentId, PersonEntity entity, IEnumerable<IRelation> relations)
        {
            entity.Countries = relations.First(r => r.Property.PropertyName == nameof(entity.Hack))
                .RelatedElementIdsAs<int>().Select(id => new PersonCountryEntity
                {
                    CountryId = id
                })
                .ToList();
            var entry = _dbContext.Persons.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override Task<PersonEntity> NewAsync(int? parentId, Type variantType = null)
        {
            return Task.FromResult(new PersonEntity());
        }

        public override int ParseKey(string id)
        {
            return int.Parse(id);
        }

        public override int? ParseParentKey(string parentId)
        {
            return int.TryParse(parentId, out var id) ? id : default(int?);
        }

        public override async Task UpdateAsync(int id, int? parentId, PersonEntity entity, IEnumerable<IRelation> relations)
        {
            var dbEntity = await _dbContext.Persons.Include(x => x.Countries).FirstOrDefaultAsync(x => x._Id == id);

            dbEntity.Name = entity.Name;

            var newCountries = relations.First(r => r.Property.PropertyName == nameof(dbEntity.Hack)).RelatedElementIdsAs<int>();

            foreach (var country in dbEntity.Countries.Where(x => !newCountries.Contains(x.CountryId.Value)).ToList())
            {
                dbEntity.Countries.Remove(country);
            }
            foreach (var countryId in newCountries.Where(id => !dbEntity.Countries.Select(x => x.CountryId.Value).Contains(id)).ToList())
            {
                dbEntity.Countries.Add(new PersonCountryEntity { CountryId = countryId });
            }

            _dbContext.Persons.Update(dbEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
