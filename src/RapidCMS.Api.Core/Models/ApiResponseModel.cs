using System.Net;
using Newtonsoft.Json;

namespace RapidCMS.Api.Core.Models;

public class ApiResponseModel
{
    public ApiResponseModel(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public ApiResponseModel(HttpStatusCode statusCode, object responseBody) : this(statusCode)
    {
        ResponseBody = JsonConvert.SerializeObject(responseBody, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        });
    }

    public HttpStatusCode StatusCode { get; set; }
    public string? ResponseBody { get; set; }
}
