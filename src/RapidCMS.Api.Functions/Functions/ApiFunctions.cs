using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.Request.Api;

namespace RapidCMS.Api.Functions.Functions
{
    public class ApiFunctions
    {
        private readonly IPresentationService _presentationService;
        private readonly IInteractionService _interactionService;

        public ApiFunctions(
            IPresentationService presentationService,
            IInteractionService interactionService)
        {
            _presentationService = presentationService;
            _interactionService = interactionService;
        }

        [FunctionName(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "/_rapidcms/{repositoryAlias}/entity/{id}")] HttpRequestData req,
            string repositoryAlias,
            string id)
        {
            try
            {
                var query = JsonConvert.DeserializeObject<ParentQueryModel>(req.Body);
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
    }
}
