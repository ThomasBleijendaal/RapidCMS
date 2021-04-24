using System.Threading.Tasks;

namespace RapidCMS.ModelMaker.Abstractions.CommandHandlers
{
    public interface ICommandHandler<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request);
    }
}
