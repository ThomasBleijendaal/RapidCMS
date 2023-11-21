using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class EditContextModel<TEntity>
    where TEntity : IEntity
{
    public EditContextModel() { }

    public EditContextModel(IEditContext<TEntity> editContext)
    {
        EntityModel = ApiBridge.EntityModel.Create(editContext.Entity);
        ParentPath = editContext.Parent?.GetParentPath()?.ToPathString();

        var container = editContext.GetRelationContainer();
        RelationContainer = new RelationContainerModel
        {
            Relations = container.Relations.Select(relation =>
            {
                return new RelationModel
                {
                    Elements = relation.RelatedElementIds,
                    PropertyName = relation.Property.PropertyName,
                    VariantAlias = AliasHelper.GetEntityVariantAlias(relation.RelatedEntityType)
                };
            })
        };
    }

    public EntityModel<TEntity> EntityModel { get; set; } = default!;
    public string? ParentPath { get; set; }

    public RelationContainerModel RelationContainer { get; set; } = default!;

    public IEnumerable<(string propertyName, string variantAlias, IEnumerable<object> elements)> GetRelations()
        => RelationContainer.Relations.Select(r => (r.PropertyName, r.VariantAlias, r.Elements));
}
