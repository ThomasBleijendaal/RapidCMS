using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using RapidCMS.Api.WebApi.Controllers;

namespace RapidCMS.Api.WebApi.Providers
{
    public class CollectionControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Add(typeof(ApiRepositoryController).GetTypeInfo());
            feature.Controllers.Add(typeof(ApiFileUploadController).GetTypeInfo());
        }
    }
}
