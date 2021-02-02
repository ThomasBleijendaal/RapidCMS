using Newtonsoft.Json;

namespace RapidCMS.Api.Functions.Models
{
    public class RequestWrapper<T> : IWrapper
    {
        [JsonProperty("Json")]
        public string Json { get; set; } = default!;

        [JsonIgnore]
        public T Value => JsonConvert.DeserializeObject<T>(Json);

        object? IWrapper.Value => Value;
    }

    public interface IWrapper
    {
        public object? Value { get; }
    }
}
