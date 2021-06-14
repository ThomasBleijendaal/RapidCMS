using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class RemoveRequest<TEntity>
        where TEntity : IEntity
    {
        public RemoveRequest(string id, string alias)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; private set; }
    }
}
