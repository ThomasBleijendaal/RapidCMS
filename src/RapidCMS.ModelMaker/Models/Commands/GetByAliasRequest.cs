using System;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByAliasRequest<TEntity>
    {
        public GetByAliasRequest(string alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public string Alias { get; set; }
    }
}
