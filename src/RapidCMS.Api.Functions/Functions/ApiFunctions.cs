using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using RapidCMS.Api.Functions.Models;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.ApiBridge;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Api.Functions.Functions
{
    public class ApiFunctions
    {
        private readonly IPresentationService _presentationService;
        private readonly IInteractionService _interactionService;
        private readonly IRepositoryTypeResolver _repositoryTypeResolver;

        public ApiFunctions(
            IPresentationService presentationService,
            IInteractionService interactionService,
            IRepositoryTypeResolver repositoryTypeResolver)
        {
            _presentationService = presentationService;
            _interactionService = interactionService;
            _repositoryTypeResolver = repositoryTypeResolver;
        }

        [FunctionName(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var id = req.Params["id"];

                var query = JsonConvert.DeserializeObject<RequestWrapper<ParentQueryModel>>(req.Body).Value;
                if (query == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor(id, repositoryAlias, query.ParentPath, query.VariantAlias),
                    UsageType = UsageType.Node | UsageType.Edit
                });

                return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(entity));
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(GetAllAsync))]
        public async Task<HttpResponseData> GetAllAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var (entityType, databaseEntityType) = _repositoryTypeResolver.GetEntityTypes(repositoryAlias);

                var query = JsonConvert.DeserializeObject<RequestWrapper<ParentQueryModel>>(req.Body).Value;
                if (query == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfParentRequestModel
                {
                    RepositoryAlias = repositoryAlias,
                    ParentPath = query.ParentPath,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery(databaseEntityType)
                });

                return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                }));
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(GetAllRelatedAsync))]
        public async Task<HttpResponseData> GetAllRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/related")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var (entityType, databaseEntityType) = _repositoryTypeResolver.GetEntityTypes(repositoryAlias);

                var query = JsonConvert.DeserializeObject<RequestWrapper<RelatedQueryModel>>(req.Body).Value;
                if (query == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = repositoryAlias,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery(databaseEntityType),
                    Related = new EntityDescriptor(query.Related.Id, query.Related.RepositoryAlias, query.Related.ParentPath, default)
                });

                return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                }));
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(GetAllNonRelatedAsync))]
        public async Task<HttpResponseData> GetAllNonRelatedAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/all/nonrelated")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var (entityType, databaseEntityType) = _repositoryTypeResolver.GetEntityTypes(repositoryAlias);

                var query = JsonConvert.DeserializeObject<RequestWrapper<RelatedQueryModel>>(req.Body).Value;
                if (query == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = repositoryAlias,
                    UsageType = UsageType.List | UsageType.Add,
                    Query = query.GetQuery(databaseEntityType),
                    Related = new EntityDescriptor(query.Related.Id, query.Related.RepositoryAlias, query.Related.ParentPath, default)
                });

                return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(new EntitiesModel<IEntity>
                {
                    Entities = EntityModel.Create(response.Entities),
                    MoreDataAvailable = response.MoreDataAvailable
                }));
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(NewAsync))]
        public async Task<HttpResponseData> NewAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/new")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];

                var query = JsonConvert.DeserializeObject<RequestWrapper<ParentQueryModel>>(req.Body).Value;
                if (query == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor(default, repositoryAlias, query.ParentPath, query.VariantAlias),
                    UsageType = UsageType.Node | UsageType.New
                });

                return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(EntityModel.Create(entity)));
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(InsertAsync))]
        public async Task<HttpResponseData> InsertAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/entity")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var (entityType, databaseEntityType) = _repositoryTypeResolver.GetEntityTypes(repositoryAlias);

                var editContextModel = GetEditContext(req, entityType);
                if (editContextModel == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(default, repositoryAlias, editContextModel.ParentPath, editContextModel.EntityModel.VariantAlias),
                    Entity = editContextModel.EntityModel.Entity,
                    EntityState = EntityState.IsNew,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => new HttpResponseData(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(persistResponse.ValidationErrors)),
                    ApiPersistEntityResponseModel insertResponse when insertResponse.NewEntity != null => new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(EntityModel.Create(insertResponse.NewEntity))),
                    _ => new HttpResponseData(HttpStatusCode.OK)
                };
            }
            catch (InvalidEntityException)
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(UpdateAsync))]
        public async Task<HttpResponseData> UpdateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var (entityType, databaseEntityType) = _repositoryTypeResolver.GetEntityTypes(repositoryAlias);
                var id = req.Params["id"];

                var editContextModel = GetEditContext(req, entityType);
                if (editContextModel == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(id, repositoryAlias, editContextModel.ParentPath, editContextModel.EntityModel.VariantAlias),
                    Entity = editContextModel.EntityModel.Entity,
                    EntityState = EntityState.IsExisting,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => new HttpResponseData(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(persistResponse.ValidationErrors)),
                    _ => new HttpResponseData(HttpStatusCode.OK)
                };
            }
            catch (InvalidEntityException)
            {
                return new HttpResponseData(HttpStatusCode.BadRequest);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(DeleteAsync))]
        public async Task<HttpResponseData> DeleteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];
                var id = req.Params["id"];

                var delete = JsonConvert.DeserializeObject<RequestWrapper<DeleteModel>>(req.Body).Value;
                if (delete == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<DeleteEntityRequestModel, ApiCommandResponseModel>(new DeleteEntityRequestModel
                {
                    Descriptor = new EntityDescriptor(id, repositoryAlias, delete.ParentPath, default)
                }, ViewState.Api);

                return new HttpResponseData(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(AddRelationAsync))]
        public async Task<HttpResponseData> AddRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];

                var relate = JsonConvert.DeserializeObject<RequestWrapper<RelateModel>>(req.Body).Value;
                if (relate == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor(relate.Related.Id, relate.Related.RepositoryAlias, relate.Related.ParentPath, default),
                    Subject = new EntityDescriptor(relate.Id, repositoryAlias, default, default),
                    Action = PersistRelatedEntityRequestModel.Actions.Add
                }, ViewState.Api);

                return new HttpResponseData(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(RemoveRelationAsync))]
        public async Task<HttpResponseData> RemoveRelationAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "_rapidcms/{repositoryAlias}/relate")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];

                var relate = JsonConvert.DeserializeObject<RequestWrapper<RelateModel>>(req.Body).Value;
                if (relate == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }

                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor(relate.Related.Id, relate.Related.RepositoryAlias, relate.Related.ParentPath, default),
                    Subject = new EntityDescriptor(relate.Id, repositoryAlias, default, default),
                    Action = PersistRelatedEntityRequestModel.Actions.Remove
                }, ViewState.Api);

                return new HttpResponseData(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        [FunctionName(nameof(ReorderAsync))]
        public async Task<HttpResponseData> ReorderAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "_rapidcms/{repositoryAlias}/reorder")] HttpRequestData req)
        {
            try
            {
                var repositoryAlias = req.Params["repositoryAlias"];

                var reorder = JsonConvert.DeserializeObject<RequestWrapper<ReorderModel>>(req.Body).Value;
                if (reorder == null)
                {
                    return new HttpResponseData(HttpStatusCode.BadRequest);
                }
                
                await _interactionService.InteractAsync<PersistReorderRequestModel, ApiCommandResponseModel>(new PersistReorderRequestModel
                {
                    BeforeId = reorder.BeforeId,
                    Subject = new EntityDescriptor(reorder.Subject.Id, repositoryAlias, reorder.Subject.ParentPath, default)
                }, ViewState.Api);

                return new HttpResponseData(HttpStatusCode.OK);
            }
            catch (NotFoundException)
            {
                return new HttpResponseData(HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpResponseData(HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                return new HttpResponseData(HttpStatusCode.InternalServerError);
            }
        }

        private static EditContextModel<IEntity>? GetEditContext(HttpRequestData req, Type entityType)
        {
            JsonConvert.DeserializeObject()

            var editContextWrapper = JsonConvert.DeserializeObject(
                req.Body,
                typeof(RequestWrapper<>).MakeGenericType(
                    typeof(EditContextModel<>).MakeGenericType(entityType)));

            var editContext = (editContextWrapper as IWrapper)?.Value;

            return editContext as EditContextModel<IEntity>;
        }
    }

    
}
