namespace RapidCMS.Api.Core.Abstractions
{
    public interface IFileHandlerResolver
    {
        IFileHandler GetApiHandler(string uploadHandlerAlias);
    }
}
