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
                CollectionAlias = related.CollectionAlias,
                Id = related.Entity.Id!
            };
        }

        public RelatedQueryModel(IRelated related, IQuery query) : this(related)
        {
            Skip = query.Skip;
            Take = query.Take;
            SearchTerm = query.SearchTerm;
        }

        public EntityDescriptorModel Related { get; set; } = default!;
    }
}
