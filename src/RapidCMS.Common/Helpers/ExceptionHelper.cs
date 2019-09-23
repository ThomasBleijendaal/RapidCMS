using System;


namespace RapidCMS.Common.Helpers
{
    public class ExceptionHelper : IExceptionHelper
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

    public interface IExceptionHelper
    {
        void StoreException(Exception ex);
        Exception? GetLatestException();
    }
}
