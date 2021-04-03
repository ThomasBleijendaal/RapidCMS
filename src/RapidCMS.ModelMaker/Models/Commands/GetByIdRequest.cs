using System;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByIdRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public GetByIdRequest(string id, string alias)
        {
            Alias = alias ?? throw new ArgumentNullException(nameof(id));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Alias { get; private set; }

        public string Id { get; private set; }
    }
}
