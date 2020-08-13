using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class RelatedQueryModel : QueryModel
    {
        public RelatedQueryModel() { }

        public RelatedQueryModel(IRelated related)
        {
            Related = new EntityDescriptorModel
            {
                RepositoryAlias = related.RepositoryAlias,
                Id = related.Entity.Id!
            };
        }

        public RelatedQueryModel(IRelated related, IQuery query) : this(related)
        {
            Skip = query.Skip;
            Take = query.Take;
            SearchTerm = query.SearchTerm;

            CollectionAlias = query.CollectionAlias;
        }

        public EntityDescriptorModel Related { get; set; } = default!;
    }
}
