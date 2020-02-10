using System;
using RapidCMS.Core.Abstractions.Services;

namespace RapidCMS.Core.Services.Exceptions
{
    internal class ExceptionService : IExceptionService
    {
        private Exception? _ex;

        public event EventHandler<Exception>? OnException;

        public void StoreException(Exception ex)
        {
            OnException?.Invoke(default, ex);

            _ex = ex ?? throw new ArgumentNullException(nameof(ex));
        }

        public Exception? GetLatestException()
        {
            return _ex;
        }
    }
}
