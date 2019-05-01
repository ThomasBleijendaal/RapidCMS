using System;

namespace RapidCMS.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSameTypeOrBaseTypeOf(this Type typeToTest, Type sameTypeOrSubClass)
        {
            return typeToTest == sameTypeOrSubClass || typeToTest == sameTypeOrSubClass.BaseType;
        }

        public static bool IsSameTypeOrDerivedFrom(this Type typeToTest, Type sameTypeOrSuperClass)
        {
            return typeToTest == sameTypeOrSuperClass || typeToTest.BaseType == sameTypeOrSuperClass;
        }
    }
}
