using System;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByAliasRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public GetByAliasRequest(string alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public string Alias { get; private set; }
    }
}
