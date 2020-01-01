using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Models.Metadata;

namespace RapidCMS.Core.Helpers
{
    internal static class PropertyMetadataHelper
    {
        /// <summary>
        /// Converts a given LambdaExpression containing MemberExpressions to getters and setters, and the name of each nested object plus the name of the property.
        /// 
        /// (Person x) => x.Company.Owner.Name becomes:
        /// getter: (object x) => (object)(((Person) x).get_Company().get_Owner().get_Name())
        /// setter: (object x, object y) => ((Person) x).get_Company().get_Owner().set_Name((string)y)
        /// objectType: Person
        /// propertyType: string
        /// name: CompanyOwnerName
        /// </summary>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>IPropertyMetadata when successful, IFullPropertyMetadata if setter is available. Returns default when unsuccessful.</returns>
        public static IPropertyMetadata? GetPropertyMetadata(LambdaExpression lambdaExpression)
        {
            Type? parameterTType = null;
            Type? parameterTPropertyType = null;

            var parameterT = Expression.Parameter(typeof(object), "x");
            var parameterTProperty = Expression.Parameter(typeof(object), "y");

            var x = lambdaExpression.Body;

            if (!(x is MemberExpression) && !(x is ParameterExpression))
            {
                parameterTType = lambdaExpression.Parameters.First().Type;
                var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;

                var getter = ConvertToGetterViaLambda(parameterT, lambdaExpression, parameterTAsType);
                if (getter == null)
                {
                    return default;
                }

                return new PropertyMetadata(lambdaExpression, lambdaExpression.Body.Type, lambdaExpression.ToString(), getter, parameterTType, GetFingerprint(lambdaExpression));
            }
            else
            {
                var isAtTail = true;
                var getNestedObjectMethods = new List<MethodInfo>();
                var names = new List<string>();

                MethodInfo? getValueMethod = null;
                MethodInfo? setValueMethod = null;

                do
                {
                    if (x is MemberExpression memberExpression)
                    {
                        var propertyInfo = memberExpression.Member as PropertyInfo;

                        if (propertyInfo == null || propertyInfo.GetGetMethod() == null)
                        {
                            return default;
                        }

                        if (isAtTail)
                        {
                            setValueMethod = propertyInfo.GetSetMethod();
                            getValueMethod = propertyInfo.GetGetMethod();
                            names.Add(propertyInfo.Name);

                            parameterTPropertyType = propertyInfo.PropertyType;

                            isAtTail = false;

                            x = memberExpression.Expression;
                        }
                        else
                        {
                            getNestedObjectMethods.Insert(0, propertyInfo.GetGetMethod()!);
                            names.Insert(0, propertyInfo.Name);

                            x = memberExpression.Expression;
                        }
                    }
                    else if (x is ParameterExpression parameterExpression)
                    {
                        parameterTType = x.Type;

                        // expression is x => x
                        if (getValueMethod == null && parameterExpression == lambdaExpression.Body)
                        {
                            return new PropertyMetadata(lambdaExpression, parameterTType, "Self", x => x, parameterTType, GetFingerprint(lambdaExpression));
                        }

                        // done, arrived at root
                        break;
                    }
                    else
                    {
                        return default;
                    }
                }
                while (true);

                if (getValueMethod == null || parameterTPropertyType == null)
                {
                    return default;
                }

                var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;
                var valueToType = Expression.Convert(parameterTProperty, parameterTPropertyType) as Expression;
                var valueToObject = Expression.Convert(Expression.Parameter(parameterTPropertyType, "z"), typeof(object));

                var instanceExpression = (getNestedObjectMethods.Count == 0)
                    ? parameterTAsType
                    : getNestedObjectMethods.Aggregate(
                        parameterTAsType,
                        (parameter, method) => Expression.Call(parameter, method));

                var name = string.Join("", names);

                var getter = ConvertToGetterViaMethod(parameterT, getValueMethod, instanceExpression);
                if (getter == null)
                {
                    return default;
                }

                var setter = setValueMethod == null ? default : ConvertToSetter(parameterT, parameterTProperty, setValueMethod, valueToType, instanceExpression);
                if (setter == null)
                {
                    return new PropertyMetadata(lambdaExpression, parameterTPropertyType, name, getter, parameterTType, GetFingerprint(lambdaExpression));
                }
                else
                {
                    return new FullPropertyMetadata(lambdaExpression, parameterTPropertyType, name, getter, setter, parameterTType, GetFingerprint(lambdaExpression));
                }
            }
        }

        /// <summary>
        /// Converts a given LambdaExpression containing expression to get value from an object.
        /// 
        /// (Person x) => $"{x.FirstName} - {x.LastName}"  becomes:
        /// getter: (object x) => (object)
        /// objectType: Person
        /// propertyType: string
        /// </summary>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, default when not.</returns>
        public static IExpressionMetadata? GetExpressionMetadata(LambdaExpression lambdaExpression)
        {
            var parameterT = Expression.Parameter(typeof(object), "x");
            var parameterTType = lambdaExpression.Parameters.First().Type;
            var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;

            var name = ((lambdaExpression.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? lambdaExpression.ToString();

            var getter = ConvertToStringGetterViaLambda(parameterT, lambdaExpression, parameterTAsType);
            if (getter == null)
            {
                return default;
            }

            return new ExpressionMetadata(name, getter);
        }

        /// <summary>
        /// Converts a given IPropertyMetadata to an IExpressionMetadata.
        /// </summary>
        /// <param name="propertyMetadata"></param>
        /// <returns></returns>
        public static IExpressionMetadata GetExpressionMetadata(IPropertyMetadata propertyMetadata)
        {
            return new ExpressionMetadata(propertyMetadata.PropertyName, (x => propertyMetadata.Getter(x)?.ToString() ?? ""));
        }

        private static Func<object, object>? ConvertToGetterViaMethod(ParameterExpression parameterT, MethodInfo getValueMethod, Expression instanceExpression)
        {
            try
            {
                var getExpression = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(Expression.Call(instanceExpression, getValueMethod), typeof(object)),
                    parameterT
                );

                return getExpression.Compile();
            }
            catch
            {
                return default;
            }
        }

        private static Func<object, object>? ConvertToGetterViaLambda(ParameterExpression parameterT, LambdaExpression lambdaExpression, Expression parameterExpression)
        {
            try
            {
                var getExpression = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Invoke(lambdaExpression, parameterExpression),
                        typeof(object)),
                    parameterT);

                return getExpression.Compile();
            }
            catch
            {
                return default;
            }
        }

        private static Func<object, string>? ConvertToStringGetterViaLambda(ParameterExpression parameterT, LambdaExpression lambdaExpression, Expression parameterExpression)
        {
            if (lambdaExpression.ReturnType != typeof(string))
            {
                return default;
            }

            try
            {
                var getExpression = Expression.Lambda<Func<object, string>>(
                    Expression.Coalesce(
                        Expression.Invoke(lambdaExpression, parameterExpression),
                        Expression.Constant(string.Empty)),
                    parameterT);

                return getExpression.Compile();
            }
            catch
            {
                return default;
            }
        }

        private static Action<object, object>? ConvertToSetter(ParameterExpression parameterT, ParameterExpression parameterTProperty, MethodInfo setValueMethod, Expression valueToType, Expression instanceExpression)
        {
            try
            {
                var setExpression = Expression.Lambda<Action<object, object>>(
                    Expression.Call(instanceExpression, setValueMethod, valueToType),
                    parameterT,
                    parameterTProperty
                );

                return setExpression.Compile();
            }
            catch
            {
                return default;
            }
        }

        private readonly static SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();

        private static string GetFingerprint(Expression expression)
        {
            var fingerprint = expression switch
            {
                LambdaExpression lambda => $"{GetFingerprint(lambda.Body)}{lambda.Body.Type.ToString()}",
                MethodCallExpression call => $"{string.Join("", call.Arguments.Select(x => GetFingerprint(x)))}{call.Type.ToString()}",
                ParameterExpression param => $"{param.IsByRef}{param.Type.ToString()}",
                MemberExpression member => $"{member.Member.Name}{GetFingerprint(member.Expression)}",
                _ => expression.Type.ToString(),
            };

            return Convert.ToBase64String(Sha1.ComputeHash(Encoding.UTF8.GetBytes(fingerprint)));
        }
    }
}
