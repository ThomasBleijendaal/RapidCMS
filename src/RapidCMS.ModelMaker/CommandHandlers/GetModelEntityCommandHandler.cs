using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers.Base;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    internal class GetModelEntityCommandHandler : BaseCommandHandler,
            ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>,
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>
    {
        public GetModelEntityCommandHandler(IModelMakerConfig config) : base(config)
        {
        }

        public async Task<EntityResponse<ModelEntity>> HandleAsync(GetByAliasRequest<ModelEntity> request)
            => new EntityResponse<ModelEntity> { Entity = await GetByIdAsync(request.Alias) };

        public async Task<EntityResponse<ModelEntity>> HandleAsync(GetByIdRequest<ModelEntity> request)
            => new EntityResponse<ModelEntity> { Entity = await GetByIdAsync(request.Alias) };

        private async Task<ModelEntity?> GetByIdAsync(string id)
            => await ReadFileToModelAsync(FileName(id));
    }
}
