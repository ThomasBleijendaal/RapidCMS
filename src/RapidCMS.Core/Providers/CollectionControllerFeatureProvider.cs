using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace RapidCMS.Core.Providers
{
    public class CollectionControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<TypeInfo> _typeInfos;

        public CollectionControllerFeatureProvider(IEnumerable<TypeInfo> typeInfos)
        {
            _typeInfos = typeInfos;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var typeInfo in _typeInfos)
            {
                feature.Controllers.Add(typeInfo);
            }
        }
    }
}
