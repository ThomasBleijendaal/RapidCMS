using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsSameTypeOrBaseTypeOf(this Type typeToTest, Type sameTypeOrSubClass)
        {
            return typeToTest == sameTypeOrSubClass || sameTypeOrSubClass.IsSubclassOf(typeToTest);
        }

        public static bool IsSameTypeOrDerivedFrom(this Type typeToTest, Type sameTypeOrSuperClass)
        {
            return typeToTest == sameTypeOrSuperClass || typeToTest.IsSubclassOf(sameTypeOrSuperClass);
        }

        public static IEnumerable<Type> GetImplementingTypes(this Type typeToFindDerivativesOf)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeToFindDerivativesOf.IsAssignableFrom(x) && x.IsClass);
        }
    }
}
