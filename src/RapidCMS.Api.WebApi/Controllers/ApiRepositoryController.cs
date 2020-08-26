using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Api.WebApi.Conventions;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Api.WebApi.Controllers
{
    public class ApiRepositoryController<TEntity, TDatabaseEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TDatabaseEntity : class
        where TRepository : IRepository
    {
        private readonly IPresentationService _presentationService;
        private readonly IInteractionService _interactionService;

        public ApiRepositoryController(
            IPresentationService presentationService,
            IInteractionService interactionService)
        {
            _presentationService = presentationService;
            _interactionService = interactionService;
        }

        public string RepositoryAlias
        {
            get => (string)ControllerContext.ActionDescriptor.Properties[CollectionControllerRouteConvention.AliasKey];
        }

        // TODO: remove all logic and put it in a general class for reuse to support things like Azure Functions

        [HttpPost("entity/{id}")]
        public async Task<ActionResult<IEntity>> GetByIdAsync(string id, [FromBody] ParentQueryModel query)
        {
            try
            {
                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        Id = id,
                        ParentPath = query.ParentPath,
                    },
                    UsageType = UsageType.Node | UsageType.Edit
                });

                return Ok(entity);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("all")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllAsync([FromBody] ParentQueryModel query)
        {
            try
            {
                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfParentRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    ParentPath = query.ParentPath,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery<TDatabaseEntity>()
                });

                return Ok(new EntitiesModel<IEntity>
                {
                    Entities = response.Entities,
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("all/related")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllRelatedAsync([FromBody] RelatedQueryModel query)
        {
            try
            {
                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor
                    {
                        RepositoryAlias = query.Related.RepositoryAlias,
                        Id = query.Related.Id
                    }
                });

                return Ok(new EntitiesModel<IEntity>
                {
                    Entities = response.Entities,
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("all/nonrelated")]
        public async Task<ActionResult<EntitiesModel<IEntity>>> GetAllNonRelatedAsync([FromBody] RelatedQueryModel query)
        {
            try
            {
                var response = await _presentationService.GetEntitiesAsync<GetEntitiesRequestModel, EntitiesResponseModel>(new GetEntitiesOfRelationRequestModel
                {
                    RepositoryAlias = RepositoryAlias,
                    UsageType = UsageType.List | UsageType.Add,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor
                    {
                        RepositoryAlias = query.Related.RepositoryAlias,
                        Id = query.Related.Id
                    }
                });

                return Ok(new EntitiesModel<IEntity>
                {
                    Entities = response.Entities,
                    MoreDataAvailable = response.MoreDataAvailable
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("new")]
        public async Task<ActionResult<IEntity>> NewAsync([FromBody] ParentQueryModel query)
        {
            try
            {
                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        ParentPath = query.ParentPath,
                        VariantAlias = query.VariantAlias
                    },
                    UsageType = UsageType.Node | UsageType.New
                });

                return Ok(entity);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("entity")]
        public async Task<ActionResult<IEntity>> InsertAsync([FromBody] EditContextModel<TEntity> editContextModel)
        {
            try
            {
                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        ParentPath = editContextModel.ParentPath
                    },
                    Entity = editContextModel.Entity,
                    EntityState = EntityState.IsNew,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => BadRequest(persistResponse.ValidationErrors),
                    ApiPersistEntityResponseModel insertResponse when insertResponse.NewEntity != null => Ok(insertResponse.NewEntity),
                    _ => Ok()
                };
            }
            catch (InvalidEntityException)
            {
                return BadRequest();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("entity/{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody] EditContextModel<TEntity> editContextModel)
        {
            try
            {
                var response = await _interactionService.InteractAsync<PersistEntityRequestModel, ApiCommandResponseModel>(new PersistEntityRequestModel
                {
                    Descriptor = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        Id = id,
                        ParentPath = editContextModel.ParentPath
                    },
                    Entity = editContextModel.Entity,
                    EntityState = EntityState.IsExisting,
                    Relations = editContextModel.GetRelations()
                }, ViewState.Api);

                return response switch
                {
                    ApiPersistEntityResponseModel persistResponse when persistResponse.ValidationErrors != null => BadRequest(persistResponse.ValidationErrors),
                    _ => Ok()
                };
            }
            catch (InvalidEntityException)
            {
                return BadRequest();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("entity/{id}")]
        public async Task<ActionResult> DeleteAsync(string id, [FromBody] DeleteModel delete)
        {
            try
            {
                await _interactionService.InteractAsync<DeleteEntityRequestModel, ApiCommandResponseModel>(new DeleteEntityRequestModel
                {
                    Descriptor = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        Id = id,
                        ParentPath = delete.ParentPath
                    }
                }, ViewState.Api);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("relate")]
        public async Task<ActionResult> AddRelationAsync([FromBody] RelateModel relate)
        {
            try
            {
                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor
                    {
                        RepositoryAlias = relate.Related.RepositoryAlias,
                        Id = relate.Related.Id
                    },
                    Subject = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        Id = relate.Id
                    },
                    Action = PersistRelatedEntityRequestModel.Actions.Add
                }, ViewState.Api);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("relate")]
        public async Task<ActionResult> RemoveRelationAsync([FromBody] RelateModel relate)
        {
            try
            {
                await _interactionService.InteractAsync<PersistRelatedEntityRequestModel, ApiCommandResponseModel>(new PersistRelatedEntityRequestModel
                {
                    Related = new EntityDescriptor
                    {
                        RepositoryAlias = relate.Related.RepositoryAlias,
                        Id = relate.Related.Id
                    },
                    Subject = new EntityDescriptor
                    {
                        RepositoryAlias = RepositoryAlias,
                        Id = relate.Id
                    },
                    Action = PersistRelatedEntityRequestModel.Actions.Remove
                }, ViewState.Api);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("reorder")]
        public async Task<ActionResult> ReorderAsync([FromBody] ReorderModel reorder)
        {
            try
            {
                await _interactionService.InteractAsync<PersistReorderRequestModel, ApiCommandResponseModel>(new PersistReorderRequestModel
                {
                    BeforeId = reorder.BeforeId,
                    Subject = new EntityDescriptor
                    {
                        Id = reorder.Subject.Id,
                        RepositoryAlias = RepositoryAlias,
                        ParentPath = reorder.Subject.ParentPath
                    }
                }, ViewState.Api);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
