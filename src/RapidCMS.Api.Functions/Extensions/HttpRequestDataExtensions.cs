using System.IO;
using System.Text;
using Microsoft.Azure.Functions.Worker.Http;
using RapidCMS.Api.Core.Models;

namespace RapidCMS.Api.Functions.Extensions
{
    internal static class HttpRequestDataExtensions
    {
        public static HttpResponseData CreateResponse(this HttpRequestData request, ApiResponseModel model)
        {
            var response = request.CreateResponse(model.StatusCode);
            if (!string.IsNullOrEmpty(model.ResponseBody))
            {
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(model.ResponseBody));
            }
            return response;
        }
    }
}
