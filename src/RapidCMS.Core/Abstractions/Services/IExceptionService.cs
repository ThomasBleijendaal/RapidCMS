using System;

namespace RapidCMS.Core.Abstractions.Services
{
    public interface IExceptionService
    {
        void StoreException(Exception ex);
        Exception? GetLatestException();
    }
}
