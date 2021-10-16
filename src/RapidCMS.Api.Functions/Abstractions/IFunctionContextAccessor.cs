using Microsoft.Azure.Functions.Worker;

namespace RapidCMS.Api.Functions.Abstractions
{
    /// <summary>
    /// PREVIEW: This class should be replaced by something first-party.
    /// </summary>
    public interface IFunctionContextAccessor
    {
        FunctionContext? FunctionContext { get; set; }
    }
}
