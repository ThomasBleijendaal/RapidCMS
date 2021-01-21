using System;
using RapidCMS.Core.Abstractions.Mediators;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class ExceptionEventArgs : IMediatorEventArgs
    {
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
