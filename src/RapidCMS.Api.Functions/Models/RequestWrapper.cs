using Newtonsoft.Json;

namespace RapidCMS.Api.Functions.Models
{
    public class RequestWrapper
    {
        [JsonProperty("Json")]
        public string Json { get; set; } = default!;
    }
}
