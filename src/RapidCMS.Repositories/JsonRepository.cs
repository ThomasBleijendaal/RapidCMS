using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.Repositories
{
    /// <summary>
    /// This generic repository loads TEntities when constructed and saves TEntities in json when deconstructed, and has some basic support for one-to-many relations.
    /// Use *only* List<TRelatedEntity> properties for relations.
    /// </summary>
    /// <typeparam name="TEntity">Entity to store</typeparam>
    public class JsonRepository<TEntity> : InMemoryRepository<TEntity>, IDisposable
        where TEntity : class, IEntity, ICloneable, new()
    {
        private readonly IDisposable _disposable;

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
                        _data = JsonConvert.DeserializeObject<Dictionary<string, List<TEntity>>>(dataFileContents);
                    }

                    var relationsFileContents = File.ReadAllText(RelationsJsonFileName());
                    if (!string.IsNullOrWhiteSpace(relationsFileContents))
                    {
                        _relations = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(relationsFileContents);
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

            _disposable = mediator.RegisterCallback<CollectionRepositoryEventArgs>(RepositoryUpdateAsync);
        }

        private static string Folder()
        {
            return "./bin/";
        }

        private static string DataJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.json";
        private static string RelationsJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.relations.json";

        private static object _lock = new object();

        private Task RepositoryUpdateAsync(object sender, CollectionRepositoryEventArgs args)
        {
            try
            {
                lock (_lock)
                {
                    File.WriteAllText(DataJsonFileName(), JsonConvert.SerializeObject(_data));
                    File.WriteAllText(RelationsJsonFileName(), JsonConvert.SerializeObject(_relations));
                }
            }
            catch { }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
