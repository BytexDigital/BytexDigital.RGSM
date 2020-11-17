using System;
using System.Linq.Expressions;

namespace BytexDigital.RGSM.Panel.Server.Common.Helpers
{
    public static class NameOf<TSource>
    {
        public static string Property(Expression<Func<TSource, object>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                    memberExpression = unaryExpression.Operand as MemberExpression;
            }

            var result = memberExpression.ToString();
            result = result.Substring(result.IndexOf('.') + 1);

            return result;
        }
    }
}
