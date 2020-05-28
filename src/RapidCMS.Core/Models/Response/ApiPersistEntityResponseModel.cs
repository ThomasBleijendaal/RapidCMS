using Microsoft.AspNetCore.Mvc.ModelBinding;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Response
{
    public class ApiPersistEntityResponseModel : ApiCommandResponseModel
    {
        public IEntity? NewEntity { get; set; }
        public ModelStateDictionary? ValidationErrors { get; set; }
    }
}
