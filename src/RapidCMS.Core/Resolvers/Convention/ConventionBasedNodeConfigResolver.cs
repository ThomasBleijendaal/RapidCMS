using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Resolvers.Convention
{
    internal class ConventionBasedNodeConfigResolver : IConventionBasedResolver<NodeConfig>
    {
        private readonly IFieldConfigResolver _fieldConfigResolver;

        public ConventionBasedNodeConfigResolver(IFieldConfigResolver fieldConfigResolver)
        {
            _fieldConfigResolver = fieldConfigResolver;
        }

        public NodeConfig ResolveByConvention(Type subject, Features features)
        {
            var nodeButtons = new List<ButtonConfig>()
            {
                new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.SaveExisting
                },
                new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.SaveNew
                },
                new DefaultButtonConfig
                {
                    ButtonType = DefaultButtonType.Delete
                }
            };
            
            var result = new NodeConfig(subject)
            { 
                BaseType = subject,
                Buttons = nodeButtons,
                Panes = new List<PaneConfig>
                {
                    new PaneConfig(subject)
                    {
                        FieldIndex = 1,
                        Fields = _fieldConfigResolver.GetFields(subject, features).ToList(),
                        VariantType = subject
                    }
                }
            };

            return result;
        }
    }
}
