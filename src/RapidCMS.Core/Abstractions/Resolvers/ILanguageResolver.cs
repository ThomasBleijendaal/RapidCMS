namespace RapidCMS.Core.Abstractions.Resolvers;

public interface ILanguageResolver
{
    string ResolveText(string originalText);
}
