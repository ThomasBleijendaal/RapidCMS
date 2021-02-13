using Microsoft.Azure.Functions.Worker.Pipeline;

namespace RapidCMS.Api.Functions.Abstractions
{
    public interface IFunctionExecutionContextAccessor
    {
        FunctionExecutionContext? FunctionExecutionContext { get; internal set; }
    }
}
