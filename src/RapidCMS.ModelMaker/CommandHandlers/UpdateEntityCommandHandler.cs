using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Abstractions.Config;
using RapidCMS.ModelMaker.CommandHandlers.Base;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    internal class UpdateModelEntityCommandHandler : BaseCommandHandler,
            ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>
    {
        public UpdateModelEntityCommandHandler(IModelMakerConfig config) : base(config)
        {
        }


        public async Task<ConfirmResponse> HandleAsync(UpdateRequest<ModelEntity> request)
        {
            await WriteModelToFileAsync(request.Entity);

            return new ConfirmResponse
            {
                Success = true
            };
        }
    }
}
