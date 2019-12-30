using System;

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
    }
}
