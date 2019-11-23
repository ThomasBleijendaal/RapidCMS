using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RapidCMS.Common.Data;

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

                var fileContents = File.ReadAllText(JsonFileName());
                if (!string.IsNullOrWhiteSpace(fileContents))
                {
                    _data = JsonConvert.DeserializeObject<Dictionary<string, List<TEntity>>>(fileContents);
                }
            }
            catch
            {
                try
                {
                    File.Delete(JsonFileName());
                }
                catch { }
            }

            UpdateJson(default);
        }

        private static string Folder()
        {
            return "./bin/";
        }

        private static string JsonFileName()
        {
            return $"{Folder()}{typeof(TEntity).FullName}.json";
        }

        private void UpdateJson(object? obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(_data);
                File.WriteAllText(JsonFileName(), json);
            }
            catch { }

            ChangeToken.RegisterChangeCallback(UpdateJson, default);
        }
    }
}
