using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class PropertyInformation : InformationBase, IInformation
    {
        private List<string> _validationAttributes = new List<string>();

        public PropertyInformation(bool hidden = false)
        {
            Hidden = hidden;
        }

        public bool Hidden { get; }

        public string? Name { get; private set; }

        public PropertyInformation HasName(string name)
        {
            Name = name;
            return this;
        }

        public string? Type { get; private set; }

        public PropertyInformation IsType(string type)
        {
            Type = type;
            return this;
        }

        public IReadOnlyList<string> ValidationAttributes => _validationAttributes;

        public PropertyInformation HasValidationAttribute(string attribute)
        {
            _namespaces.Add((Use.Entity, "System.ComponentModel.DataAnnotations"));

            _validationAttributes.Add(attribute);
            return this;
        }

        public PropertyInformation IsRequired(bool isRequired)
        {
            if (isRequired)
            {
                _validationAttributes.Insert(0, "[Required]");
            }
            return this;
        }

        public bool RelatedToOneEntity { get; set; }
        public bool RelatedToManyEntities { get; set; }
        public string? RelatedCollectionAlias { get; set; }
        public string? DataCollectionExpression { get; set; }

        public PropertyInformation IsRelation(bool relatedToOneEntity, bool relatedToManyEntities, string? relatedCollectionAlias, string? dataCollectionExpression)
        {
            RelatedToOneEntity = relatedToOneEntity;
            RelatedToManyEntities = relatedToManyEntities;
            RelatedCollectionAlias = relatedCollectionAlias;
            DataCollectionExpression = dataCollectionExpression;

            if (RelatedToManyEntities)
            {
                _namespaces.Add((Use.Entity, "System.Collections.Generic"));
            }

            return this;
        }

        public bool IsTitleOfEntity { get; set; }

        public PropertyInformation IsTitle(bool isTitle)
        {
            IsTitleOfEntity = isTitle;
            return this;
        }

        public string? EditorType { get; private set; }

        public PropertyInformation UsesEditor(string type)
        {
            EditorType = type;
            return this;
        }

        public Use Uses { get; private set; } = Use.Collection | Use.Context | Use.Entity | Use.Repository;

        public PropertyInformation UseFor(Use use)
        {
            Uses = use;
            return this;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Type) &&
                !string.IsNullOrEmpty(EditorType);
        }

        public IEnumerable<string> NamespacesUsed(Use use)
        {
            foreach (var @namespace in _namespaces.Where(x => x.use.HasFlag(use)).Select(x => x.@namespace).Distinct())
            {
                yield return @namespace;
            }
        }
    }
}
