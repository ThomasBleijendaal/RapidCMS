using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Authorization
{
    public static class Operations
    {
        // TODO: consolidate (Add + Pick similair etc)
        public static OperationAuthorizationRequirement None = new OperationAuthorizationRequirement { Name = nameof(CrudType.None) };
        public static OperationAuthorizationRequirement List = new OperationAuthorizationRequirement { Name = nameof(CrudType.List) };
        public static OperationAuthorizationRequirement View = new OperationAuthorizationRequirement { Name = nameof(CrudType.View) };
        public static OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = nameof(CrudType.Create) };
        public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = nameof(CrudType.Read) };
        public static OperationAuthorizationRequirement Insert = new OperationAuthorizationRequirement { Name = nameof(CrudType.Insert) };
        public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = nameof(CrudType.Update) };
        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = nameof(CrudType.Delete) };
        public static OperationAuthorizationRequirement Add = new OperationAuthorizationRequirement { Name = nameof(CrudType.Add) };
        public static OperationAuthorizationRequirement Remove = new OperationAuthorizationRequirement { Name = nameof(CrudType.Remove) };
        public static OperationAuthorizationRequirement Pick = new OperationAuthorizationRequirement { Name = nameof(CrudType.Pick) };
        public static OperationAuthorizationRequirement Refresh = new OperationAuthorizationRequirement { Name = nameof(CrudType.Refresh) };

        public static OperationAuthorizationRequirement GetOperationForCrudType(CrudType type)
        {
            return type switch
            {
                CrudType.None => None,
                CrudType.View => View,
                CrudType.Create => Create,
                CrudType.Read => Read,
                CrudType.Insert => Insert,
                CrudType.Update => Update,
                CrudType.Delete => Delete,
                CrudType.Add => Add,
                CrudType.Remove => Remove,
                CrudType.Pick => Pick,
                CrudType.Refresh => Refresh,
                _ => throw new InvalidOperationException($"Operation of type {type} is not supported.")
            };
        }
    }
}
