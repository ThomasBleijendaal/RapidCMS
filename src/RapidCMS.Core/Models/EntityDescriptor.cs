using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models
{
    public record EntityDescriptor(string? Id, string? RepositoryAlias, IParent? Parent, string? VariantAlias)
    {
        public static EntityDescriptor Create(FormEditContext context) 
            => new(context.Entity.Id, context.RepositoryAlias, context.Parent, context.EntityVariantAlias);
    }
}
