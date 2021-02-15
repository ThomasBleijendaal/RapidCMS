using System.Threading;
using Microsoft.Azure.Functions.Worker.Pipeline;
using RapidCMS.Api.Functions.Abstractions;

namespace RapidCMS.Api.Functions.Accessors
{
    // this accessor is temporary and should be replaced with something first party
    // TODO: test this accessor in concurrent function requests
    internal class FunctionExecutionContextAccessor : IFunctionExecutionContextAccessor
    {
        private static AsyncLocal<ContextHolder> _contextCurrent = new AsyncLocal<ContextHolder>();

        FunctionExecutionContext? IFunctionExecutionContextAccessor.FunctionExecutionContext
        {
            get
            {
                return _contextCurrent.Value?.Context;
            }
            set
            {
                var holder = _contextCurrent.Value;
                if (holder != null)
                {
                    holder.Context = null!;
                }

                if (value != null)
                {
                    _contextCurrent.Value = new ContextHolder { Context = value };
                }
            }
        }

        private class ContextHolder
        {
            public FunctionExecutionContext Context = default!;
        }
    }
}
