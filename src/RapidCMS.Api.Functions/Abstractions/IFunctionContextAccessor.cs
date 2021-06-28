using Microsoft.Azure.Functions.Worker;

namespace RapidCMS.Api.Functions.Abstractions
{
    public interface IFunctionContextAccessor
    {
        FunctionContext? FunctionExecutionContext { get; set; }
    }
}
