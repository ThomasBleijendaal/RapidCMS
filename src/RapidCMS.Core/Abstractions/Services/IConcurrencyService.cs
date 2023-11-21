using System;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Services;

public interface IConcurrencyService
{
    Task EnsureCorrectConcurrencyAsync(Func<Task> function);
    Task<T> EnsureCorrectConcurrencyAsync<T>(Func<Task<T>> function);
}
