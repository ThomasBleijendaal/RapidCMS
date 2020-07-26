using System;
using System.Linq;
using System.Security.Cryptography;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Helpers
{
    public static class AliasHelper
    {
        private readonly static SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();

        public static string GetFileUploaderAlias(Type handlerType)
        {
            var type = (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition().Name.StartsWith("ApiFileUpload"))
                ? handlerType.GetGenericArguments().FirstOrDefault()
                : handlerType;

            return type?.Name.ToSha1Base64String() ?? "unknown-file-handler";
        }

        public static string GetRepositoryAlias(Type repositoryType)
        {
            var name = repositoryType?.FullName;
            var alias = name?.ToSha1Base64String() ?? "unknown-repository";

            return alias;
        }
    }
}
