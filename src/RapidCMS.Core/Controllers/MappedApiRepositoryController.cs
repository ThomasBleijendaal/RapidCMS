using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Conventions;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.State;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Controllers
{
    public class MappedApiRepositoryController<TEntity, TDatabaseEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TDatabaseEntity : class
        where TRepository : BaseMappedRepository<TEntity, TDatabaseEntity>
    {
        private readonly IPresentationService _presentationService;
        private readonly IInteractionService _interactionService;

        public MappedApiRepositoryController(
            IPresentationService presentationService,
            IInteractionService interactionService)
        {
            _presentationService = presentationService;
            _interactionService = interactionService;
        }

        public string CollectionAlias
        {
            get => (string)ControllerContext.ActionDescriptor.Properties[CollectionControllerRouteConvention.AliasKey];
        }

        // TODO: validation?
        // TODO: parentId + IQuery + variant + pagination
        // TODO: remove all logic and put it in a general class for reuse to support things like Azure Functions
        // TODO: mapped variation?

        [HttpPost("entity/{id}")]
        public async Task<ActionResult<IEntity>> GetByIdAsync(string id, [FromBody] ParentQueryModel query)
        {
            try
            {
                var entity = await _presentationService.GetEntityAsync<GetEntityRequestModel, IEntity>(new GetEntityRequestModel
                {
                    Subject = new EntityDescriptor
                    {
                        CollectionAlias = CollectionAlias,
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
                    CollectionAlias = CollectionAlias,
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
                    CollectionAlias = CollectionAlias,
                    UsageType = UsageType.List | UsageType.Edit,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor
                    {
                        CollectionAlias = query.Related.CollectionAlias,
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
                    CollectionAlias = CollectionAlias,
                    UsageType = UsageType.List | UsageType.Add,
                    Query = query.GetQuery<TDatabaseEntity>(),
                    Related = new EntityDescriptor
                    {
                        CollectionAlias = query.Related.CollectionAlias,
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
                        CollectionAlias = CollectionAlias,
                        ParentPath = query.ParentPath,
                        VariantAlias = query.VariantTypeName
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
                        CollectionAlias = CollectionAlias,
                        ParentPath = editContextModel.ParentPath
                    },
                    Entity = editContextModel.Entity,
                    EntityState = EntityState.IsNew
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
                        CollectionAlias = CollectionAlias,
                        Id = id,
                        ParentPath = editContextModel.ParentPath
                    },
                    Entity = editContextModel.Entity,
                    EntityState = EntityState.IsExisting
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
                        CollectionAlias = CollectionAlias,
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
                        CollectionAlias = relate.Related.CollectionAlias,
                        Id = relate.Related.Id
                    },
                    Subject = new EntityDescriptor
                    {
                        CollectionAlias = CollectionAlias,
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
                        CollectionAlias = relate.Related.CollectionAlias,
                        Id = relate.Related.Id
                    },
                    Subject = new EntityDescriptor
                    {
                        CollectionAlias = CollectionAlias,
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
                        CollectionAlias = CollectionAlias,
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
