using System;
using System.Linq.Expressions;

namespace R8.Lib
{
    public static class ExpressionReflections
    {
        /// <summary>
        /// Returns possible <see cref="MemberExpression"/> from given lambda expression.
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static MemberExpression GetMemberExpression(this LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null)
                throw new ArgumentNullException(nameof(lambdaExpression));

            return lambdaExpression.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)lambdaExpression.Body;
        }
    }
}