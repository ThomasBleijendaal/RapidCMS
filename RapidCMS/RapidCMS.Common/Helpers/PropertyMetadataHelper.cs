using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using RapidCMS.Common.Models;

#nullable enable

[assembly: InternalsVisibleTo("RapidCMS.Common.Tests")]
namespace RapidCMS.Common.Helpers
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
        /// <exception cref="ArgumentException">Thrown when given LamdaExpression cannot be converted to a getter and setter.</exception>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, null when not.</returns>
        public static IPropertyMetadata GetPropertyMetadata(LambdaExpression lambdaExpression)
        {
            return GetExpressionMetadata(lambdaExpression) as IPropertyMetadata 
                ?? throw new ArgumentException($"Given expression {lambdaExpression.ToString()} cannot be converted to Getter and Setter.");
        }

        /// <summary>
        /// Converts a given LambdaExpression containing expression to get value from an object.
        /// 
        /// (Person x) => $"{x.FirstName} - {x.LastName}"  becomes:
        /// getter: (object x) => (object)
        /// objectType: Person
        /// propertyType: string
        /// 
        /// When possible (LambdaExpression is a MemberExpression), it will return IPropertyMetadata similair to GetPropertyMetadata.
        /// </summary>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, null when not.</returns>
        public static IExpressionMetadata GetExpressionMetadata(LambdaExpression lambdaExpression)
        {
            try
            {
                var isAtTail = true;

                Type? parameterTType = null;
                Type? parameterTPropertyType = null;

                var parameterT = Expression.Parameter(typeof(object), "x");
                var parameterTProperty = Expression.Parameter(typeof(object), "y");
                Expression? parameterTAsType = null;

                MethodInfo? setValueMethod = null;
                MethodInfo? getValueMethod = null;
                var names = new List<string>();
                var getNestedObjectMethods = new List<MethodInfo>();

                var x = lambdaExpression.Body;

                do
                {
                    if (x is MemberExpression memberExpression)
                    {
                        var propertyInfo = memberExpression.Member as PropertyInfo;
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
                            getNestedObjectMethods.Insert(0, propertyInfo.GetGetMethod());
                            names.Insert(0, propertyInfo.Name);

                            x = memberExpression.Expression;
                        }
                    }
                    else if (x is ParameterExpression parameterExpression)
                    {
                        parameterTType = x.Type;

                        // done, arrived at root
                        break;
                    }
                    else if (x is MethodCallExpression methodCallExpression && isAtTail)
                    {
                        var del = lambdaExpression.Compile();
                        

                        getValueMethod = methodCallExpression.Method;
                        //parameterTType = getValueMethod.;

                        //if (parameterTType != null)
                        //{
                            //parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;

                            return new ExpressionMetadata
                            {
                                PropertyType = methodCallExpression.Type,
                                Getter = ConvertToGetter(parameterT, getValueMethod, parameterT)
                            };
                        //}
                    }
                    else
                    {
                        throw new Exception("Failed to interpret given LambdaExpression");
                    }
                }
                while (true);

                if (getValueMethod == null || setValueMethod == null)
                {
                    throw new Exception("Failed to process given LambdaExpression");
                }

                parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;
                var valueToType = Expression.Convert(parameterTProperty, parameterTPropertyType) as Expression;
                var valueToObject = Expression.Convert(Expression.Parameter(parameterTPropertyType, "z"), typeof(object));

                var instanceExpression = (getNestedObjectMethods.Count == 0)
                    ? parameterTAsType
                    : getNestedObjectMethods.Aggregate(
                        parameterTAsType,
                        (parameter, method) => Expression.Call(parameter, method));

                var setter = ConvertToSetter(parameterT, parameterTProperty, setValueMethod, valueToType, instanceExpression);
                var getter = ConvertToGetter(parameterT, getValueMethod, instanceExpression);
                var name = string.Join("", names);

                return new PropertyMetadata
                {
                    ObjectType = parameterTType,
                    Getter = getter,
                    Setter = setter,
                    PropertyName = name,
                    PropertyType = parameterTPropertyType
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Func<object, object> ConvertToGetter(ParameterExpression parameterT, MethodInfo getValueMethod, Expression instanceExpression)
        {
            var getExpression = Expression.Lambda<Func<object, object>>(
                Expression.Convert(Expression.Call(instanceExpression, getValueMethod), typeof(object)),
                parameterT
            );

            return getExpression.Compile();
        }

        private static Action<object, object> ConvertToSetter(ParameterExpression parameterT, ParameterExpression parameterTProperty, MethodInfo setValueMethod, Expression valueToType, Expression instanceExpression)
        {
            var setExpression = Expression.Lambda<Action<object, object>>(
                Expression.Call(instanceExpression, setValueMethod, valueToType),
                parameterT,
                parameterTProperty
            );

            return setExpression.Compile();
        }
    }
}
