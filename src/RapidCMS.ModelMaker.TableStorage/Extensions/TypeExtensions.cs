using System;
using RapidCMS.Core.Extensions;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.TableStorage.Extensions
{
    public static class TypeExtensions
    {
        public static string GetTableName(this Type modelType)
            => modelType == typeof(ModelEntity) ? "models" : "data";

        public static string GetPartitionKey(this Type modelType)
            => modelType.Name.ToUrlFriendlyString();
    }
}
