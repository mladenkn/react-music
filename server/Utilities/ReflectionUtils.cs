using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities
{
    public static class ReflectionUtils
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var memberExpression = GetMemberExpr(propertyLambda);

            if (!(memberExpression.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            return propInfo;
        }
        public static string GetPropertyPath<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var memberExpression = GetMemberExpr(propertyLambda);

            if (!(memberExpression.Member is PropertyInfo))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            return memberExpression.ToString();
        }

        public static void ForEachPropertyDeep(object o, Action<PropertyInfo> consumeProperty)
        {
            var props = o.GetType().GetProperties();
            if (props.GetType().IsPrimitive())
                return;
            foreach (var propertyInfo in props) 
                consumeProperty(propertyInfo);
        }

        public static T ShallowCopy<T>(T sourceObject) where T : new()
        {
            var destinationObject = new T();
            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var propInfo in props)
            {
                var propValue = propInfo.GetValue(sourceObject);
                propInfo.SetValue(destinationObject, propValue);
            }
            return destinationObject;
        }

        private static MemberExpression GetMemberExpr<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            switch (propertyLambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return ((UnaryExpression)propertyLambda.Body).Operand as MemberExpression;
                case ExpressionType.MemberAccess:
                    return propertyLambda.Body as MemberExpression;
                default:
                    throw new Exception("Unsupported expression type in ReflectionUtils.GetMemberExpr");
            }
        }
    }
}
