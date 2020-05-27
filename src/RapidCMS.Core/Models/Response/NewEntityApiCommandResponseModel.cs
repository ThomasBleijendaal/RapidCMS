using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Response
{
    public class NewEntityApiCommandResponseModel : ApiCommandResponseModel
    { 
        public IEntity? NewEntity { get; set; }
    }
}
