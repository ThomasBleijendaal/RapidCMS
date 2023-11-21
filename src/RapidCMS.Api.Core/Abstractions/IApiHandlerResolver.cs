namespace RapidCMS.Api.Core.Abstractions;

public interface IApiHandlerResolver
{
    IApiHandler GetApiHandler(string repositoryAlias);
}
