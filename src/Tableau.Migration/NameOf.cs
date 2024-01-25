using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class for chained "nameof" functionality.
    /// </summary>
    internal static class NameOf
    {
        /// <summary>
        /// <para>
        /// Builds the member chain string for the given expression.
        /// </para>
        /// <para>
        /// For example, given the expression () => myObject.MyInnerObject.MyProperty, the string "myObject.MyInnerObject.MyProperty" will be returned.
        /// </para>
        /// </summary>
        /// <param name="expression">The expression to generate the member chain string for.</param>
        /// <returns></returns>
        public static string Build(Expression<Func<object?>> expression) => Build(expression.Body);

        private static string Build(Expression? expression)
        {
            // Handle reference types
            if (expression is MemberExpression memberExpression)
            {
                return String.Join(
                    ".",
                    new[]
                    {
                        Build(memberExpression.Expression),
                        memberExpression.Member.Name
                    }
                    .Where(v => !String.IsNullOrEmpty(v)));
            }
            // Handle value types
            else if (expression is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                return Build(unaryExpression.Operand);
            }

            return String.Empty;
        }
    }
}
