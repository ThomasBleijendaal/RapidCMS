using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Common.Data;
using TestLibrary.Data;
using TestLibrary.Entities;

namespace TestLibrary.Repositories
{
    public class PersonRepository : BaseStructRepository<int, int, PersonEntity>
    {
        private readonly TestDbContext _dbContext;

        public PersonRepository(TestDbContext dbContext, SemaphoreSlim semaphoreSlim) : base(semaphoreSlim)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(int id, int? parentId)
        {
            var entry = new PersonEntity { _Id = id };
            _dbContext.Persons.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<PersonEntity>> GetAllAsync(int? parentId, IQuery<PersonEntity> query)
        {
            var persons = await _dbContext.Persons
                .Include(x => x.Countries).ThenInclude(x => x.Country)
                .OrderBy(x => x._Id)
                .Skip(query.Skip)
                .Take(query.Take + 1)
                .AsNoTracking()
                .ToListAsync();

            query.HasMoreData(persons.Count > query.Take);

            return persons.Take(query.Take);
        }

        public override async Task<PersonEntity> GetByIdAsync(int id, int? parentId)
        {
            return await _dbContext.Persons.Include(x => x.Countries).ThenInclude(x => x.Country).AsNoTracking().FirstOrDefaultAsync(x => x._Id == id);
        }

        public override async Task<PersonEntity> InsertAsync(int? parentId, PersonEntity entity, IRelationContainer? relations)
        {
            entity.Countries = relations?.GetRelatedElementIdsFor<CountryEntity, int>()?.Select(id => new PersonCountryEntity { CountryId = id }).ToList() ?? new List<PersonCountryEntity>();

            var entry = _dbContext.Persons.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override Task<PersonEntity> NewAsync(int? parentId, Type? variantType = null)
        {
            return Task.FromResult(new PersonEntity { Countries = new List<PersonCountryEntity>() });
        }

        public override int ParseKey(string id)
        {
            return int.Parse(id);
        }

        public override int? ParseParentKey(string? parentId)
        {
            return int.TryParse(parentId, out var id) ? id : default(int?);
        }

        public override async Task UpdateAsync(int id, int? parentId, PersonEntity entity, IRelationContainer? relations)
        {
            var dbEntity = await _dbContext.Persons.Include(x => x.Countries).FirstOrDefaultAsync(x => x._Id == id);

            dbEntity.Name = entity.Name;

            var newCountries = relations?.GetRelatedElementIdsFor<CountryEntity, int>();

            if (newCountries != null)
            {
                foreach (var country in dbEntity.Countries.Where(x => !newCountries.Contains(x.CountryId.Value)).ToList())
                {
                    dbEntity.Countries.Remove(country);
                }
                foreach (var countryId in newCountries.Where(id => !dbEntity.Countries.Select(x => x.CountryId.Value).Contains(id)).ToList())
                {
                    dbEntity.Countries.Add(new PersonCountryEntity { CountryId = countryId });
                }
            }

            _dbContext.Persons.Update(dbEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
