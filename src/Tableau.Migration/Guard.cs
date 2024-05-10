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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing helper methods for argument checking
    /// </summary>
    internal static class Guard
    {
        #region - AgainstNull -

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static object AgainstNull([NotNull] object? value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName);

            return value; //Expression<Func<object?>> expression
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static object AgainstNull([NotNull] object? value, Expression<Func<object?>> expression)
        {
            return AgainstNull(value, NameOf.Build(expression));
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static T AgainstNull<T>([NotNull] T? value, string paramName)
            where T : class
        {
            if (value is null)
                throw new ArgumentNullException(paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static T AgainstNull<T>([NotNull] T? value, string paramName)
            where T : struct
        {
            if (value is null)
                throw new ArgumentNullException(paramName);

            return value.Value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static T AgainstNull<T>([NotNull] T? value, Expression<Func<object?>> expression)
            where T : class
        {
            return AgainstNull(value, NameOf.Build(expression));
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the supplied object is null
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static T AgainstNull<T>([NotNull] T? value, Expression<Func<object?>> expression)
            where T : struct
        {
            return AgainstNull(value, NameOf.Build(expression));
        }

        #endregion

        #region - AgainstNullOrEmpty -

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null or empty
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static string AgainstNullOrEmpty([NotNull] string? value, string paramName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null or empty
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static string AgainstNullOrEmpty([NotNull] string? value, Expression<Func<object?>> expression)
        {
            return AgainstNullOrEmpty(value, NameOf.Build(expression));
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied collection is null or empty
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static IEnumerable AgainstNullOrEmpty([NotNull] IEnumerable? value, string paramName)
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied collection is null or empty
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static IEnumerable AgainstNullOrEmpty([NotNull] IEnumerable? value, Expression<Func<object?>> expression)
        {
            return AgainstNullOrEmpty(value, NameOf.Build(expression));
        }

        #endregion

        #region - AgainstNullEmptyOrWhiteSpace -

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null, empty, or whitespace
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static string AgainstNullEmptyOrWhiteSpace([NotNull] string? value, string paramName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null, empty, or whitespace
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static string AgainstNullEmptyOrWhiteSpace([NotNull] string? value, Expression<Func<object?>> expression)
        {
            return AgainstNullEmptyOrWhiteSpace(value, NameOf.Build(expression));
        }

        #endregion

        #region - AgainstNullOrWhiteSpace -

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null or whitespace
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static string AgainstNullOrWhiteSpace([NotNull] string? value, string paramName)
        {
            if (String.IsNullOrWhiteSpace(value) && value != String.Empty)
                throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied string is null or whitespace
        /// </summary>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static string AgainstNullOrWhiteSpace([NotNull] string? value, Expression<Func<object?>> expression)
        {
            return AgainstNullOrWhiteSpace(value, NameOf.Build(expression));
        }

        #endregion

        #region - AgainstDefaultValue -

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied value is the type's default value
        /// </summary>
        /// <typeparam name="TValue">the type of value</typeparam>
        /// <param name="value">the value to evaluate</param>
        /// <param name="paramName">the name of the parameter</param>
        public static TValue AgainstDefaultValue<TValue>(TValue value, string paramName)
        {
            if (value is null || default(TValue)!.Equals(value))
                throw new ArgumentException($"{paramName} cannot be type {typeof(TValue).Name}'s default value.", paramName);

            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the supplied value is the type's default value
        /// </summary>
        /// <typeparam name="TValue">the type of value</typeparam>
        /// <param name="value">the value to evaluate</param>
        /// <param name="expression">the expression for the parameter</param>
        public static TValue AgainstDefaultValue<TValue>(TValue value, Expression<Func<object?>> expression)
        {
            return AgainstDefaultValue(value, NameOf.Build(expression));
        }

        #endregion
    }
}
