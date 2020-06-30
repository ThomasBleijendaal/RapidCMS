using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;

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
        public JsonRepository(IServiceProvider serviceProvider) : base(serviceProvider)
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

            UpdateJson(default);
        }

        private static string Folder()
        {
            return "./bin/";
        }

        private static string DataJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.json";
        private static string RelationsJsonFileName() => $"{Folder()}{typeof(TEntity).FullName}.relations.json";

        private static object _lock = new object();

        private void UpdateJson(object? obj)
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

            ChangeToken.RegisterChangeCallback(UpdateJson, default);
        }
    }
}
