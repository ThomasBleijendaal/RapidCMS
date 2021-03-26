using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    internal class ModelEntity : IEntity
    {
        public string? Id { get; set; }
    }

    internal class PropertyModel
    {
        public string? Name { get; set; }
    }

    internal class ShortStringPropertyModel : PropertyModel
    {
        public string EditorAlias { get; set; }
        public List<PropertyValidation> Validations { get; set; }
    }

    internal class PropertyValidation
    {
        public string Alias { get; set; }
        public IValidatorConfig Config { get; set; }
    }


    internal class PropertyModelDescriptor<TPropertyModel> where TPropertyModel : PropertyModel
    {
        public string Alias { get; set; }

        public Type Type { get; set; }

        public List<string> ValidatorAliases { get; set; }

        public List<PropertyEditorDescriptor> Editors { get; set; }
    }

    internal class PropertyValidationDescriptor<TValidator>
    {

    }

    internal class PropertyValidationDescriptor
    {
        public string Alias { get; set; }
        public string ConfigAlias { get; set; }
        public string EditorAlias { get; set; }
    }

    internal class PropertyEditorDescriptor
    {
        public string Alias { get; set; }
    }

    public interface IValidator
    {
        Task<bool> IsValid(object? value, IValidatorConfig validatorConfig);
        Task<string> ErrorMessage(IValidatorConfig validatorConfig);
    }

    public abstract class BaseValidator<TValue, TValidatorConfig> : IValidator 
        where TValidatorConfig : IValidatorConfig
    {
        public Task<string> ErrorMessage(IValidatorConfig validatorConfig)
        {
            if (validatorConfig is TValidatorConfig config)
            {
                return ErrorMessage(config);
            }

            return Task.FromResult("Unknown error");
        }

        public Task<bool> IsValid(object? value, IValidatorConfig validatorConfig)
        {
            if (value is TValue validationValue && validatorConfig is TValidatorConfig config)
            {
                return IsValid(validationValue, config);
            }

            return Task.FromResult(false);
        }

        protected abstract Task<bool> IsValid(TValue? value, TValidatorConfig validatorConfig);
        protected abstract Task<string> ErrorMessage(TValidatorConfig validatorConfig);
    }

    public class MinLengthValidator : BaseValidator<string, MinLengthValidationConfig>
    {
        protected override Task<string> ErrorMessage(MinLengthValidationConfig validatorConfig)
        {
            return Task.FromResult($"The input has to be at least {validatorConfig.MinLength} characters long.");
        }

        protected override Task<bool> IsValid(string? value, MinLengthValidationConfig validatorConfig)
        {
            return Task.FromResult(value?.Length > validatorConfig.MinLength);
        }
    }

    public class LimitedOptionsValidator : BaseValidator<string, LimitedOptionsValidationConfig>
    {
        protected override Task<string> ErrorMessage(LimitedOptionsValidationConfig validatorConfig)
        {
            return Task.FromResult($"The input must be one of these values: {string.Join(", ", validatorConfig.Options)}.");
        }

        protected override Task<bool> IsValid(string? value, LimitedOptionsValidationConfig validatorConfig)
        {
            return Task.FromResult(validatorConfig.Options.Contains(value ?? string.Empty));
        }
    }

    public interface IValidator<TValidatorConfig>
    {

    }

    public interface IValidatorConfig
    {

    }

    public class MinLengthValidationConfig : IValidatorConfig
    {
        public int MinLength { get; set; }
    }

    public class LimitedOptionsValidationConfig : IValidatorConfig
    {
        public List<string> Options { get; set; }
    }



}
