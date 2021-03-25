using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerEntity : IEntity
    {
        public string? Id { get; set; }

        public Dictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();

        public T? Get<T>(string property) => Get(property) is T data ? data : default;
        public object? Get(string property) => Data.TryGetValue(property, out var data) ? data : default;

        public void Set(string property, object? value) => Data[property] = value;
    }


}
