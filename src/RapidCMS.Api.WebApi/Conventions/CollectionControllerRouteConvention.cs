using Microsoft.AspNetCore.Mvc.ApplicationModels;
using RapidCMS.Api.WebApi.Controllers;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Api.WebApi.Conventions
{
    public class CollectionControllerRouteConvention : IControllerModelConvention
    {
        public const string RouteTemplatePrefix = "/api/";
        
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.In(typeof(ApiRepositoryController), typeof(ApiFileUploadController)))
            {
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = RouteTemplatePrefix
                    }
                });
            }
        }
    }
}
