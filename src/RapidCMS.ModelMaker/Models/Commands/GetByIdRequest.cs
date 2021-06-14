using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByIdRequest<TEntity>
        where TEntity : IEntity
    {
        public GetByIdRequest(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; private set; }
    }
}
