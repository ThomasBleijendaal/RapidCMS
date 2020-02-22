using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Models.Config
{
    internal class PageConfig : IPageConfig
    {
        public PageConfig(string name, string icon, string alias)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Alias { get; set; }

        IEnumerable<CustomTypeRegistrationConfig> IPageConfig.SectionRegistrations => SectionRegistrations;

        private List<CustomTypeRegistrationConfig> SectionRegistrations { get; set; } = new List<CustomTypeRegistrationConfig>();

        public IPageConfig AddSection(string collectionAlias, bool edit = false)
        {
            SectionRegistrations.Add(
                new CustomTypeRegistrationConfig(
                    typeof(ICollectionConfig),
                    new Dictionary<string, object> {
                        {
                            "InitialState",
                            new PageStateModel
                            {
                                PageType = PageType.Collection,
                                UsageType = edit ? UsageType.Edit : UsageType.View,
                                CollectionAlias = collectionAlias
                            }
                        }
                    }));

            return this;
        }

        public IPageConfig AddSection(Type customSectionType)
        {
            if (!customSectionType.IsSameTypeOrDerivedFrom(typeof(ComponentBase)))
            {
                throw new InvalidOperationException($"{nameof(customSectionType)} must be derived of {nameof(ComponentBase)}.");
            }

            SectionRegistrations.Add(new CustomTypeRegistrationConfig(customSectionType));

            return this;
        }
    }
}
