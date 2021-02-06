using Newtonsoft.Json;

namespace RapidCMS.Api.Functions.Models
{
    public class JsonRequestWrapper
    {
        [JsonProperty("Json")]
        public string Json { get; set; } = default!;
    }
}
