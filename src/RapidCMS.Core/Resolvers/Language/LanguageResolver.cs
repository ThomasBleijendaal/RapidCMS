using RapidCMS.Core.Abstractions.Resolvers;

namespace RapidCMS.Core.Resolvers.Language
{
    internal class LanguageResolver : ILanguageResolver
    {
        public string ResolveText(string originalText)
        {
            return originalText;
        }
    }
}
