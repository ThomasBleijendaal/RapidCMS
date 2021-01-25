using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository loads TEntities when constructed and saves TEntities in json when deconstructed, and has some basic support for one-to-many relations.
    /// Use *only* List<TRelatedEntity> properties for relations.
    /// </summary>
    /// <typeparam name="TEntity">Entity to store</typeparam>
    public class JsonRepository<TEntity> : InMemoryRepository<TEntity>
        where TEntity : class, IEntity, ICloneable, new()
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public JsonRepository(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
            try
            {
                if (!Directory.Exists(Folder()))
                {
                    Directory.CreateDirectory(Folder());
                }

                lock (_lock)
                {
                    var dataFileContents = File.ReadAllText(DataJsonFileName());
                    if (!string.IsNullOrWhiteSpace(dataFileContents))
                    {
                        _data = JsonConvert.DeserializeObject<Dictionary<string, List<TEntity>>>(dataFileContents, _jsonSerializerSettings) ?? new();
                    }

                    var relationsFileContents = File.ReadAllText(RelationsJsonFileName());
                    if (!string.IsNullOrWhiteSpace(relationsFileContents))
                    {
                        _relations = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(relationsFileContents, _jsonSerializerSettings) ?? new();
                    }
                }
            }
            catch
            {
                try
                {
                    File.Delete(DataJsonFileName());
                    File.Delete(RelationsJsonFileName());
                }
                catch { }
            }

            UpdateJson();
        }

        private static string Folder()
        {
            return "./bin/";
        }

        private static string DataJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.json";
        private static string RelationsJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.relations.json";

        private static object _lock = new object();

        private void UpdateJson()
        {
            try
            {
                lock (_lock)
                {
                    File.WriteAllText(DataJsonFileName(), JsonConvert.SerializeObject(_data, _jsonSerializerSettings));
                    File.WriteAllText(RelationsJsonFileName(), JsonConvert.SerializeObject(_relations, _jsonSerializerSettings));
                }
            }
            catch { }
        }

        public override async Task AddAsync(IRelated related, string id)
        {
            await base.AddAsync(related, id);
            UpdateJson();
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            await base.DeleteAsync(id, parent);
            UpdateJson();
        }

        public override async Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
        {
            var entity = await base.InsertAsync(editContext);
            UpdateJson();
            return entity;
        }

        public override async Task RemoveAsync(IRelated related, string id)
        {
            await base.RemoveAsync(related, id);
            UpdateJson();
        }

        public override async Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            await base.ReorderAsync(beforeId, id, parent);
            UpdateJson();
        }

        public override async Task UpdateAsync(IEditContext<TEntity> editContext)
        {
            await base.UpdateAsync(editContext);
            UpdateJson();
        }
    }
}
