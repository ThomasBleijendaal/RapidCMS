using System;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers.Base;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    internal class PublishModelEntityCommandHandler : BaseCommandHandler,
            ICommandHandler<PublishRequest<ModelEntity>, ConfirmResponse>
    {
        private readonly ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> _commandHandler;

        public PublishModelEntityCommandHandler(
            ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>> commandHandler,
            IModelMakerConfig config) : base(config)
        {
            _commandHandler = commandHandler;
        }

        public async Task<ConfirmResponse> HandleAsync(PublishRequest<ModelEntity> request)
        {
            if (string.IsNullOrEmpty(request.Entity.Id))
            {
                throw new InvalidOperationException();
            }

            // TODO: move publish logic to some external resolver + check how publishing should be supported model maker wide
            // there should be some publishing logic that activates a lot of command handlers to update models etc.
            if (request.Entity is ModelEntity modelEntity)
            {
                modelEntity.PublishedProperties = modelEntity.DraftProperties;
            }

            await WriteModelToFileAsync(request.Entity);

            return new ConfirmResponse
            {
                Success = true
            };
        }
    }
}
