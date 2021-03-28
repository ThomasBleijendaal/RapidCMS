using System;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class GetByIdRequest<TEntity>
    {
        public GetByIdRequest(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; set; }
    }
}
