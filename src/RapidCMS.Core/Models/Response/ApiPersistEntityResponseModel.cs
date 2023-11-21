using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Response;

public class ApiPersistEntityResponseModel : ApiCommandResponseModel
{
    public IEntity? NewEntity { get; set; }
    public ModelStateDictionary? ValidationErrors { get; set; }
}
