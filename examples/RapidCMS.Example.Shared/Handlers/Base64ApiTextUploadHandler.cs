using System.Net.Http;
using RapidCMS.Core.Handlers;

namespace RapidCMS.Example.Shared.Handlers
{
    public class Base64ApiTextUploadHandler : ApiFileUploadHandler<Base64TextFileUploadHandler>, ITextUploadHandler
    {
        public Base64ApiTextUploadHandler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }
    }
}
