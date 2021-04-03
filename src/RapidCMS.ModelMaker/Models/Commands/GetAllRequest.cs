using System;
using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetAllRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public GetAllRequest(string? alias)
        {
            Alias = alias;
        }

        public string? Alias { get; private set; }
    }
}
