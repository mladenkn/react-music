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
