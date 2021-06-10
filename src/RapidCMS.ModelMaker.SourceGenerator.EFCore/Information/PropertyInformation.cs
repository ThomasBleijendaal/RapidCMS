using System.Collections.Generic;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class PropertyInformation : InformationBase, IInformation
    {
        private List<string> _validationAttributes = new List<string>();

        public PropertyInformation()
        {
        }

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

        public PropertyInformation IsRelation(bool relatedToOneEntity, bool relatedToManyEntities)
        {
            RelatedToOneEntity = relatedToOneEntity;
            RelatedToManyEntities = relatedToManyEntities;

            if (RelatedToManyEntities)
            {
                _namespaces.Add("System.Collections.Generic");
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

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && 
                !string.IsNullOrEmpty(Type) &&
                !string.IsNullOrEmpty(EditorType);
        }

        public IEnumerable<string> NamespacesUsed()
        {
            if (_validationAttributes.Count > 0)
            {
                yield return "System.ComponentModel.DataAnnotations";
            }

            foreach (var @namespace in _namespaces)
            {
                yield return @namespace;
            }
        }
    }
}
