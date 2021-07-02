using System;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsSameTypeOrBaseTypeOf(this Type typeToTest, Type sameTypeOrSubClass)
            => typeToTest == sameTypeOrSubClass || sameTypeOrSubClass.IsSubclassOf(typeToTest);

        public static bool IsSameTypeOrDerivedFrom(this Type typeToTest, Type sameTypeOrSuperClass) 
            => typeToTest == sameTypeOrSuperClass || typeToTest.IsSubclassOf(sameTypeOrSuperClass);

        public static bool HasInterface(this Type typeToTest, Type @interface) 
            => typeToTest.GetInterfaces().Any(x => x == @interface);

        public static bool HasGenericInterface(this Type typeToTest, Type @interface)
            => @interface.IsGenericType && 
                @interface.IsGenericTypeDefinition && 
                typeToTest.GetInterfaces().Where(x => x.IsGenericType).Any(x => x.GetGenericTypeDefinition() == @interface);

        public static IEnumerable<Type> GetImplementingTypes(this Type typeToFindDerivativesOf) 
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeToFindDerivativesOf.IsAssignableFrom(x) && x.IsClass);

        public static TAttribute? GetCustomAttribute<TAttribute>(this Type type)
                where TAttribute : Attribute 
            => type.GetCustomAttributes(typeof(TAttribute), true)?.FirstOrDefault() as TAttribute;

        public static IEnumerable<Type> GetSubTypes(this Type type)
            => type.Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(type));
    }
}
