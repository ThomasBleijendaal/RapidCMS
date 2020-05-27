using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RapidCMS.Core.Conventions
{
    public class CollectionControllerRouteConvention : IControllerModelConvention
    {
        public const string RouteTemplatePrefix = "/api/_rapidcms/";
        public const string CollectionAliasKey = "__rapidCMSCollectionAlias";

        private readonly IReadOnlyDictionary<TypeInfo, string> _controllers;

        public CollectionControllerRouteConvention(IReadOnlyDictionary<TypeInfo, string> controllers)
        {
            _controllers = controllers;
        }

        public void Apply(ControllerModel controller)
        {
            var type = controller.ControllerType;

            if (_controllers.TryGetValue(type, out var collectionAlias))
            {
                controller.Properties.Add(CollectionAliasKey, collectionAlias);

                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = $"{RouteTemplatePrefix}{collectionAlias}"
                    }
                });
            }
        }
    }
}
