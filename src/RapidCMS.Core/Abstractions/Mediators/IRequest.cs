namespace RapidCMS.Core.Abstractions.Mediators
{
    public interface IRequest : IRequest<Unit>
    {

    }

    public interface IRequest<TResponse> : IMediatorServiceRequest
    {

    }
}
