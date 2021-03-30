using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.Validation.Config;

namespace RapidCMS.ModelMaker.CommandHandlers
{
    public class InMemoryModelEntityCommandHandler :
        ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>,
        ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>,
        ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>,
        ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>,
        ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>,
        ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>
    {
        internal static List<ModelEntity> MODELS = new List<ModelEntity>
        {
            new ModelEntity
            {
                // TODO: icon + color
                Id = "1",
                Alias = "dynamicmodels",
                Name = "Dynamic model",
                PublishedProperties = new List<PropertyModel>
                {
                    new PropertyModel
                    {
                        // TODO: description, details, placeholder
                        Id = "1,",
                        EditorAlias = "textbox",
                        Name = "Name",
                        Alias = "name",
                        IsTitle = true,
                        PropertyAlias = "shortstring",
                        Validations = new List<PropertyValidationModel>
                        {
                            new PropertyValidationModel<MinLengthValidationConfig>
                            {
                                Id = "1",
                                Alias = "minlength",
                                Config = new MinLengthValidationConfig
                                {
                                    MinLength = 10
                                }
                            }
                        }
                    }
                }
            }
        };


        Task<ConfirmResponse> ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>.HandleAsync(RemoveRequest<ModelEntity> request)
        {
            MODELS.RemoveAll(x => x.Id == request.Id);
            return Task.FromResult(new ConfirmResponse { Success = true });
        }

        Task<EntitiesResponse<ModelEntity>> ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>.HandleAsync(GetAllRequest<ModelEntity> request)
        {
            return Task.FromResult(new EntitiesResponse<ModelEntity> { Entities = MODELS });
        }

        Task<EntityResponse<ModelEntity>> ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>.HandleAsync(GetByIdRequest<ModelEntity> request)
        {
            return Task.FromResult(new EntityResponse<ModelEntity> { Entity = MODELS.FirstOrDefault(x => x.Id == request.Id) });
        }

        Task<EntityResponse<ModelEntity>> ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>.HandleAsync(GetByAliasRequest<ModelEntity> request)
        {
            return Task.FromResult(new EntityResponse<ModelEntity> { Entity = MODELS.FirstOrDefault(x => x.Alias == request.Alias) });
        }

        Task<EntityResponse<ModelEntity>> ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>.HandleAsync(InsertRequest<ModelEntity> request)
        {
            var entity = request.Entity;
            entity.Id = $"{MODELS.Count + 1}";
            MODELS.Add(entity);
            return Task.FromResult(new EntityResponse<ModelEntity> { Entity = entity });
        }

        Task<ConfirmResponse> ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>.HandleAsync(UpdateRequest<ModelEntity> request)
        {
            return Task.FromResult(new ConfirmResponse { Success = true });
        }
    }
}
