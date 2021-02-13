using Newtonsoft.Json;

namespace RapidCMS.Api.Functions.Models
{
    public class BytesRequestWrapper
    {
        [JsonProperty("Bytes")]
        public string Bytes { get; set; } = default!;
    }
}
