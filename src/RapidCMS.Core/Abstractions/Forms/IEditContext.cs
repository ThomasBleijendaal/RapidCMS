using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;

namespace RapidCMS.Core.Abstractions.Forms
{
    public interface IEditContext
    {
        /// <summary>
        /// Checks whether the given data is valid according to the model's validation attributes.
        /// 
        /// IMPORTANT: 
        /// 
        /// In the case of using ServerSide/WebAssembly RapidCMS: The EditContext will only validate the properties 
        /// that were touched by the form and that were visible (included) on the form.
        /// 
        /// In the case of using Api RapidCMS: The EditContext will only validate the properties that were
        /// modified by the Api call, so [Required] parameters won't always be caught (the Api cannot be sure whether
        /// the omited property was included on the form.
        /// 
        /// Use EnforceCompleteValidation to strictly validate the complete model, which will also check properties
        /// that were not included on the form or in the Api call.
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Checks all the properties of the model (regardless of their inclusion) and will throw when the model is not valid.
        /// </summary>
        /// <exception cref="InvalidEntityException">Thrown when the given entity has validation errors.</exception>
        void EnforceCompleteValidation();

        /// <summary>
        /// Gets the ModelStateDirectory containing all validation errors, to be used in BadRequest() responses etc.
        /// </summary>
        ModelStateDictionary ValidationErrors { get; }
    }

    public interface IEditContext<TEntity> : IEditContext
        where TEntity : IEntity
    {
        /// <summary>
        /// Indicates how the entity was used.
        /// </summary>
        UsageType UsageType { get; }
        /// <summary>
        /// Indicates the state of the entity, new or existing.
        /// </summary>
        EntityState EntityState { get; }

        /// <summary>
        /// The subject
        /// </summary>
        TEntity Entity { get; }
        /// <summary>
        /// Possible parent(s) of the subject
        /// </summary>
        IParent? Parent { get; }

        IRelationContainer GetRelationContainer();

        /// <summary>
        /// Checks whether the given property was modified by the form.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        bool? IsModified<TValue>(Expression<Func<TEntity, TValue>> property);
        /// <summary>
        /// Checks whether the given property was modified by the form.
        /// </summary>
        /// <param name="propertyName">Name of the direct property of TEntity.</param>
        /// <returns></returns>
        bool? IsModified(string propertyName);

        /// <summary>
        /// Checks whether the given property is valid.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        bool? IsValid<TValue>(Expression<Func<TEntity, TValue>> property);
        /// <summary>
        /// Checks whether the given property is valid.
        /// </summary>
        /// <param name="propertyName">Name of the direct property of TEntity.</param>
        /// <returns></returns>
        bool? IsValid(string propertyName);

        /// <summary>
        /// Checks whether the given property was validated by the EditContext.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        bool? WasValidated<TValue>(Expression<Func<TEntity, TValue>> property);
        /// <summary>
        /// Checks whether the given property was validated by the EditContext.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool? WasValidated(string propertyName);

        /// <summary>
        /// Validates the given property and returns whether this property was valid.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        bool? Validate<TValue>(Expression<Func<TEntity, TValue>> property);
    }
}
