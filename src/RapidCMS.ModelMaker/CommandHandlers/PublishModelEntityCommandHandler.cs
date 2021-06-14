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

            await WriteModelToFileAsync(request.Entity);

            return new ConfirmResponse
            {
                Success = true
            };
        }
    }
}
