using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Authorization
{
    public static class Operations
    {
        /// <summary>
        /// Read-only viewing of entity
        /// </summary>
        public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = nameof(Read) };

        /// <summary>
        /// Creating a new instance of entity
        /// </summary>
        public static OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = nameof(Create) };

        /// <summary>
        /// Modifying an existing instance of entity
        /// </summary>
        public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = nameof(Update) };

        /// <summary>
        /// Deleting an existing instance of entity
        /// </summary>
        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = nameof(Delete) };

        /// <summary>
        /// Adding a relation to an existing entity
        /// </summary>
        public static OperationAuthorizationRequirement Add = new OperationAuthorizationRequirement { Name = nameof(Add) };

        /// <summary>
        /// Removing an existing relation to an existing entity
        /// </summary>
        public static OperationAuthorizationRequirement Remove = new OperationAuthorizationRequirement { Name = nameof(Remove) };

        internal static OperationAuthorizationRequirement GetOperationForUsageType(UsageType type)
        {
            return (type & ~(UsageType.Root | UsageType.NotRoot | UsageType.Node | UsageType.List)) switch
            {
                UsageType.Add => Add,
                UsageType.Edit => Update,
                UsageType.List => Read,
                UsageType.New => Create,
                UsageType.Pick => Add,
                UsageType.View => Read,
                _ => throw new InvalidOperationException($"Operation of type {type} is not supported.")
            };
        }
    }
}
