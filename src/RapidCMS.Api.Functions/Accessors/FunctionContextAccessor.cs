using System.Threading;
using Microsoft.Azure.Functions.Worker;
using RapidCMS.Api.Functions.Abstractions;

namespace RapidCMS.Api.Functions.Accessors
{
    // PREVIEW: this accessor is temporary and should be replaced with something first party
    // TODO: test this accessor in concurrent function requests
    internal class FunctionContextAccessor : IFunctionContextAccessor
    {
        private static AsyncLocal<ContextHolder> _contextCurrent = new AsyncLocal<ContextHolder>();

        FunctionContext? IFunctionContextAccessor.FunctionExecutionContext
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
            public FunctionContext Context = default!;
        }
    }
}
