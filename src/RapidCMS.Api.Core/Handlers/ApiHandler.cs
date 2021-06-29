using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RapidCMS.Api.Core.Abstractions;
using RapidCMS.Api.Core.Models;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Converters;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.ApiBridge;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Api.Core.Handlers
{
    internal class ApiHandler<TEntity, TDatabaseEntity, TRepository> : IApiHandler
        where TEntity : class, IEntity
        where TDatabaseEntity : class
        where TRepository : IRepository
    {
        private readonly IPresentationService _presentationService;
        private readonly IInteractionService _interactionService;
        private readonly ILogger<ApiHandler<TEntity, TDatabaseEntity, TRepository>> _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ApiHandler(
            IPresentationService presentationService,
            IInteractionService interactionService,
            ILogger<ApiHandler<TEntity, TDatabaseEntity, TRepository>> logger)
        {
            _presentationService = presentationService;
            _interactionService = interactionService;
            _logger = logger;
            _jsonSerializerSettings = new JsonSerializerSettings();

            if (Activator.CreateInstance(typeof(EntityModelJsonConverter<>).MakeGenericType(typeof(TEntity))) is JsonConverter jsonConverter)
            {
                _jsonSerializerSettings.Converters.Add(jsonConverter);
            }
            else
            {
                throw new InvalidOperationException($"Could not create {nameof(EntityModelJsonConverter<IEntity>)} for {typeof(TEntity).Name}");
            }
        }

        public string RepositoryAlias => AliasHelper.GetRepositoryAlias(typeof(TRepository));

        public async Task<ApiResponseModel> AddRelationAsync(ApiRequestModel request)
        {
            try
            {
                var relate = JsonConvert.DeserializeObject<RelateModel>(request.Body ?? "", _jsonSerializerSettings);
                if (relate == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor(relate.Related.Id, relate.Related.RepositoryAlias, relate.Related.ParentPath, default),
                    Subject = new EntityDescriptor(relate.Id, RepositoryAlias, default, default),
                    Action = PersistRelatedEntityRequestModel.Actions.Add
                }, ViewState.Api);

                return new ApiResponseModel(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(AddRelationAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> DeleteAsync(ApiRequestModel request)
        {
            try
            {
                var delete = JsonConvert.DeserializeObject<DeleteModel>(request.Body ?? "", _jsonSerializerSettings);
                if (delete == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<DeleteEntityRequestModel, ApiCommandResponseModel>(new DeleteEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(request.Id, RepositoryAlias, delete.ParentPath, default)
                }, ViewState.Api);

                return new ApiResponseModel(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(DeleteAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> GetAllAsync(ApiRequestModel request)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<ParentQueryModel>(request.Body ?? "", _jsonSerializerSettings);
                if (query == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfParentRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    ParentPath = query.ParentPath,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery<TDatabaseEntity>()
                });

                return new ApiResponseModel(HttpStatusCode.OK, new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetAllAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> GetAllNonRelatedAsync(ApiRequestModel request)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<RelatedQueryModel>(request.Body ?? "", _jsonSerializerSettings);
                if (query == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    UsageType = UsageType.List | UsageType.Add,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor(query.Related.Id, query.Related.RepositoryAlias, query.Related.ParentPath, default)
                });

                return new ApiResponseModel(HttpStatusCode.OK, new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetAllNonRelatedAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> GetAllRelatedAsync(ApiRequestModel request)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<RelatedQueryModel>(request.Body ?? "", _jsonSerializerSettings);
                if (query == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor(query.Related.Id, query.Related.RepositoryAlias, query.Related.ParentPath, default)
                });

                return new ApiResponseModel(HttpStatusCode.OK, new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetAllRelatedAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> GetByIdAsync(ApiRequestModel request)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<ParentQueryModel>(request.Body ?? "", _jsonSerializerSettings);
                if (query == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor(request.Id, RepositoryAlias, query.ParentPath, query.VariantAlias),
                    UsageType = UsageType.Node | UsageType.Edit
                });

                return new ApiResponseModel(HttpStatusCode.OK, EntityModel.Create(entity));
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetByIdAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> InsertAsync(ApiRequestModel request)
        {
            try
            {
                var editContextModel = JsonConvert.DeserializeObject<EditContextModel<TEntity>>(request.Body ?? "", _jsonSerializerSettings);
                if (editContextModel == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(default, RepositoryAlias, editContextModel.ParentPath, editContextModel.EntityModel.VariantAlias),
                    Entity = editContextModel.EntityModel.Entity,
                    EntityState = EntityState.IsNew,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => new ApiResponseModel(HttpStatusCode.BadRequest, persistResponse.ValidationErrors),
                    ApiPersistEntityResponseModel insertResponse when insertResponse.NewEntity != null => new ApiResponseModel(HttpStatusCode.OK, EntityModel.Create(insertResponse.NewEntity)),
                    _ => new ApiResponseModel(HttpStatusCode.OK)
                };
            }
            catch (InvalidEntityException)
            {
                return new ApiResponseModel(HttpStatusCode.BadRequest);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(InsertAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> NewAsync(ApiRequestModel request)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<ParentQueryModel>(request.Body ?? "", _jsonSerializerSettings);
                if (query == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor(default, RepositoryAlias, query.ParentPath, query.VariantAlias),
                    UsageType = UsageType.Node | UsageType.New
                });

                return new ApiResponseModel(HttpStatusCode.OK, EntityModel.Create(entity));
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(NewAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> RemoveRelationAsync(ApiRequestModel request)
        {
            try
            {
                var relate = JsonConvert.DeserializeObject<RelateModel>(request.Body ?? "", _jsonSerializerSettings);
                if (relate == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor(relate.Related.Id, relate.Related.RepositoryAlias, relate.Related.ParentPath, default),
                    Subject = new EntityDescriptor(relate.Id, RepositoryAlias, default, default),
                    Action = PersistRelatedEntityRequestModel.Actions.Remove
                }, ViewState.Api);

                return new ApiResponseModel(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(RemoveRelationAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> ReorderAsync(ApiRequestModel request)
        {
            try
            {
                var reorder = JsonConvert.DeserializeObject<ReorderModel>(request.Body ?? "", _jsonSerializerSettings);
                if (reorder == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<PersistReorderRequestModel, ApiCommandResponseModel>(new PersistReorderRequestModel
                {
                    BeforeId = reorder.BeforeId,
                    Subject = new EntityDescriptor(reorder.Subject.Id, RepositoryAlias, reorder.Subject.ParentPath, default)
                }, ViewState.Api);

                return new ApiResponseModel(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(ReorderAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponseModel> UpdateAsync(ApiRequestModel request)
        {
            try
            {
                var editContextModel = JsonConvert.DeserializeObject<EditContextModel<TEntity>>(request.Body ?? "", _jsonSerializerSettings);
                if (editContextModel == null)
                {
                    return new ApiResponseModel(HttpStatusCode.BadRequest);
                }

                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(request.Id, RepositoryAlias, editContextModel.ParentPath, editContextModel.EntityModel.VariantAlias),
                    Entity = editContextModel.EntityModel.Entity,
                    EntityState = EntityState.IsExisting,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => new ApiResponseModel(HttpStatusCode.BadRequest, persistResponse.ValidationErrors),
                    _ => new ApiResponseModel(HttpStatusCode.OK)
                };
            }
            catch (InvalidEntityException)
            {
                return new ApiResponseModel(HttpStatusCode.BadRequest);
            }
            catch (NotFoundException)
            {
                return new ApiResponseModel(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseModel(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(UpdateAsync)}.");

                return new ApiResponseModel(HttpStatusCode.InternalServerError);
            }
        }
    }
}
