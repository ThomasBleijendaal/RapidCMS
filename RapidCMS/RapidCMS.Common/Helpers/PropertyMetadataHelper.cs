using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Helpers
{
    // TODO: only return interface instead of PropertyMetadata
    internal static class PropertyMetadataHelper
    {
        /// <summary>
        /// Converts a given LambdaExpression containing MemberExpressions to getters and setters, and the name of each nested object plus the name of the property.
        /// 
        /// (Person x) => x.Company.Owner.Name becomes:
        /// getter: (object x) => (object)(((Person) x).get_Company().get_Owner().get_Name())
        /// setter: (object x, object y) => ((Person) x).get_Company().get_Owner().set_Name((string)y)
        /// name: CompanyOwnerName
        /// </summary>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, null when not.</returns>
        internal static PropertyMetadata Create(LambdaExpression lambdaExpression)
        {
            try
            {
                var isAtTail = true;

                var parameterT = Expression.Parameter(typeof(object), "x");
                Type parameterTType = null;
                var parameterTProperty = Expression.Parameter(typeof(object), "y");
                Type parameterTPropertyType = null;

                MethodInfo setValueMethod = null;
                MethodInfo getValueMethod = null;
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
                    else
                    {
                        throw new Exception("Failed to interpret given LambdaExpression");
                    }
                }
                while (true);

                var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;
                var valueToType = Expression.Convert(parameterTProperty, parameterTPropertyType) as Expression;
                var valueToObject = Expression.Convert(Expression.Parameter(parameterTPropertyType, "z"), typeof(object));

                var instanceExpression = (getNestedObjectMethods.Count == 0)
                    ? parameterTAsType
                    : getNestedObjectMethods.Aggregate(
                        parameterTAsType,
                        (parameter, method) => Expression.Call(parameter, method));


                var setExpression =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Call(instanceExpression, setValueMethod, valueToType),
                        parameterT,
                        parameterTProperty
                    );

                var getExpression =
                    Expression.Lambda<Func<object, object>>(
                        Expression.Convert(Expression.Call(instanceExpression, getValueMethod), typeof(object)),
                        parameterT
                    );

                var name = string.Join("", names);
                var setter = setExpression.Compile();
                var getter = getExpression.Compile();

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
    }
}
