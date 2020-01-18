using System;
using RapidCMS.Core.Abstractions.Services;

namespace RapidCMS.Core.Services.Exceptions
{
    internal class ExceptionService : IExceptionService
    {
        private Exception? _ex;

        public void StoreException(Exception ex)
        {
            _ex = ex ?? throw new ArgumentNullException(nameof(ex));
        }

        public Exception? GetLatestException()
        {
            return _ex;
        }
    }
}
