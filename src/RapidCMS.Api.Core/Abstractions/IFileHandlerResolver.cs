namespace RapidCMS.Api.Core.Abstractions;

public interface IFileHandlerResolver
{
    IFileHandler GetFileHandler(string uploadHandlerAlias);
}
