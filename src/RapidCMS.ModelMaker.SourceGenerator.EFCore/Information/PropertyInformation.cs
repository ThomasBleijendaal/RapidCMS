using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class PropertyInformation : InformationBase, IInformation
    {
        public PropertyInformation(bool hidden = false)
        {
            Hidden = hidden;
        }

        public bool Hidden { get; }

        public string? Name { get; private set; }

        public string? PascalCaseName => ValidPascalCaseName(Name);

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

        public PropertyInformation IsRequired(bool isRequired)
        {
            if (isRequired)
            {
                Validations.Add(new ValidationInformation("NotNull"));
            }
            return this;
        }

        public Relation Relation { get; set; }
        public string? RelatedCollectionAlias { get; set; }
        public string? RelatedPropertyName { get; set; }
        public string? DataCollectionExpression { get; set; }

        public PropertyInformation IsRelation(
            Relation relation,
            string? relatedCollectionAlias,
            string? relatedPropertyName,
            string? dataCollectionExpression)
        {
            Relation = relation;
            RelatedCollectionAlias = relatedCollectionAlias;
            RelatedPropertyName = relatedPropertyName;
            DataCollectionExpression = dataCollectionExpression;

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

        public bool IncludeInListView { get; private set; }

        public PropertyInformation ShouldBeDisplayedInListView(bool include)
        {
            IncludeInListView = include;

            return this;
        }

        public List<ValidationInformation> Validations { get; private set; } = new List<ValidationInformation>();

        public PropertyInformation AddValidation(ValidationInformation validation)
        {
            Validations.Add(validation);

            return this;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Type) &&
                !string.IsNullOrEmpty(EditorType) &&
                Validations.All(x => x.IsValid());
        }

        public IEnumerable<string> NamespacesUsed(Use use)
        {
            if (use == Use.Entity && Relation.HasFlag(Relation.ToMany))
            {
                yield return "System.Collections.Generic";
            }


            foreach (var @namespace in _namespaces.Where(x => x.use.HasFlag(use)).Select(x => x.@namespace).Distinct())
            {
                yield return @namespace;
            }

            foreach (var @namespace in Validations.SelectMany(x => x.NamespacesUsed(use)))
            {
                yield return @namespace;
            }
        }
    }
}
