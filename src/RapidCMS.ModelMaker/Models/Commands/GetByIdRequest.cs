using System;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByIdRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public GetByIdRequest(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; set; }
    }
}
