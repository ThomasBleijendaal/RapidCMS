using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request;

public class RelatedQueryModel : QueryModel
{
    public RelatedQueryModel() { }

    public RelatedQueryModel(IRelated related)
    {
        Related = new EntityDescriptorModel
        {
            RepositoryAlias = related.RepositoryAlias,
            Id = related.Entity.Id!,
            ParentPath = related.Parent?.GetParentPath()?.ToPathString()
        };
    }

    public RelatedQueryModel(IRelated related, IView view) : this(related)
    {
        Skip = view.Skip;
        Take = view.Take;
        SearchTerm = view.SearchTerm;

        CollectionAlias = view.CollectionAlias;
    }

    public EntityDescriptorModel Related { get; set; } = default!;
}
