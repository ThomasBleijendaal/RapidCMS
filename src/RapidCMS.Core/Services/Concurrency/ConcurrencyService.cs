using System;
using System.Threading;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Services;

namespace RapidCMS.Core.Services.Concurrency
{
    internal class ConcurrencyService : IConcurrencyService
    {
        private readonly SemaphoreSlim _semaphore;

        public ConcurrencyService(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public async Task EnsureCorrectConcurrencyAsync(Func<Task> function)
        {
            await _semaphore.WaitAsync();

            try
            {
                await function.Invoke();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> EnsureCorrectConcurrencyAsync<T>(Func<Task<T>> function)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await function.Invoke();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
