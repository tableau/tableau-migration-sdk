//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
        /// <returns>A string representing the specified expression.</returns>
        public static string Build<T>(Expression<Func<T>> expression) => Build(expression.Body);

        /// <summary>
        /// <para>
        /// Builds the member chain string for the given expression.
        /// </para>
        /// <para>
        /// For example, given the expression (x) => x.MyInnerObject.MyProperty, the string "MyInnerObject.MyProperty" will be returned.
        /// </para>
        /// </summary>
        /// <param name="expression">The expression to generate the member chain string for.</param>
        /// <returns>A string representing the specified expression.</returns>
        public static string Build<T>(Expression<Func<T, object?>> expression) => Build(expression.Body);

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
