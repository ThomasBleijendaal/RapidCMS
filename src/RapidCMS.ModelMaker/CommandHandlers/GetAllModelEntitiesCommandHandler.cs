using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers.Base;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    internal class GetAllModelEntitiesCommandHandler : BaseCommandHandler,
            ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>
    {
        public GetAllModelEntitiesCommandHandler(IModelMakerConfig config) : base(config)
        {
        }

        public async Task<EntitiesResponse<ModelEntity>> HandleAsync(GetAllRequest<ModelEntity> request)
        {
            if (!Directory.Exists(_config.ModelFolder))
            {
                Directory.CreateDirectory(_config.ModelFolder);
            }

            var files = Directory.GetFiles(_config.ModelFolder, "*.json")
                .OrderBy(x => x);

            var results = new List<ModelEntity>();

            foreach (var fileName in files)
            {
                if (await ReadFileToModelAsync(fileName) is ModelEntity entity)
                {
                    results.Add(entity);
                }
            }

            return new EntitiesResponse<ModelEntity> { Entities = results };
        }
    }
}
