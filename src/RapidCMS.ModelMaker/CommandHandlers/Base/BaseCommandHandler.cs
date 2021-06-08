using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.CommandHandlers.Base
{
    internal abstract class BaseCommandHandler
    {
        protected readonly IModelMakerConfig _config;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };


        public BaseCommandHandler(IModelMakerConfig config)
        {
            _config = config;
        }

        protected string FileName(string id) => $"{_config.ModelFolder}{id.Replace("::", ".")}.json";

        protected async Task<ModelEntity?> ReadFileToModelAsync(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return default;
            }

            var json = await File.ReadAllTextAsync(fileName);
            return JsonConvert.DeserializeObject<ModelEntity>(json, _jsonSerializerSettings);
        }

        protected async Task WriteModelToFileAsync(ModelEntity entity)
        {
            var json = JsonConvert.SerializeObject(entity, _jsonSerializerSettings);
            var fileName = FileName(entity.Id ?? throw new InvalidOperationException("Cannot save entity without Id"));

            await File.WriteAllTextAsync(fileName, json);
        }
    }
}
