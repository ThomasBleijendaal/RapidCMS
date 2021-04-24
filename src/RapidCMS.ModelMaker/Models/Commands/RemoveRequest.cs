using System;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class RemoveRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public RemoveRequest(string id, string alias)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public string Alias { get; private set; }
        public string Id { get; private set; }
    }
}
