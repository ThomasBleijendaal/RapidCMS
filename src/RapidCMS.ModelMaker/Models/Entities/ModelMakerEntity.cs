using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Entities
{
    internal class ModelMakerEntity : IEntity
    {
        public string? Id { get; set; }

        public string ModelAlias { get; set; } = default!;

        public Dictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();

        public T? Get<T>(string property) => Get(property) is T data ? data : default;
        public object? Get(string property) => Data.TryGetValue(property, out var data) ? data : default;

        public void Set(string property, object? value) => Data[property] = value;
    }
}
