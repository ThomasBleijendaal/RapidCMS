﻿//using System;
//using System.Collections.Generic;
//using RapidCMS.ModelMaker.Abstractions.Entities;
//using RapidCMS.ModelMaker.Enums;

//namespace RapidCMS.ModelMaker.Models.Entities
//{
//    public class ModelMakerEntity : IPublishableModelMakerEntity
//    {
//        public string? Id { get; set; }

//        public string Alias { get; set; } = default!;
//        public PublishState State { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime PublishedAt { get; set; }
//        public DateTime UpdatedAt { get; set; }

//        public Dictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();

//        public T? Get<T>(string property) => Get(property) is T data ? data : default;
//        public object? Get(string property) => Data.TryGetValue(property, out var data) ? data : default;

//        public void Set(string property, object? value) => Data[property] = value;
//    }
//}
