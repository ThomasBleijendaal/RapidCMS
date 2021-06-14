using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers.Base;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    internal class InsertModelEntityCommandHandler : BaseCommandHandler,
            ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>
    {
        public InsertModelEntityCommandHandler(IModelMakerConfig config) : base(config)
        {
        }

        public async Task<EntityResponse<ModelEntity>> HandleAsync(InsertRequest<ModelEntity> request)
        {
            await WriteModelToFileAsync(request.Entity);

            return new EntityResponse<ModelEntity> { Entity = request.Entity };
        }
    }
}
