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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration
{
    /// <summary>
    /// Class representing the result of an operation.
    /// </summary>
    internal record Result : IResult
    {
        /// <summary>
        /// Gets whether the operation was successful.
        /// </summary>
        public virtual bool Success { get; }

        /// <summary>
        /// Gets any exceptions encountered during the operation.
        /// </summary>
        public virtual IImmutableList<Exception> Errors { get; }

        /// <summary>
        /// Creates a new <see cref="Result"/> instance.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected Result(bool success, params Exception[] errors)
            : this(success, errors is null ? ImmutableArray<Exception>.Empty : errors)
        { }

        /// <summary>
        /// Creates a new <see cref="Result"/> instance.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected Result(bool success, IEnumerable<Exception> errors)
        {
            Success = success;
            Errors = errors.ToImmutableArray();

            Validate();
        }

        /// <summary>
        /// Creates a new <see cref="Result"/> instance.
        /// </summary>
        /// <param name="copyResult">An <see cref="IResult"/> to copy values from.</param>
        protected Result(IResult copyResult)
            : this(copyResult.Success, copyResult.Errors)
        { }

        /// <summary>
        /// Creates a new <see cref="Result"/> instance for successful operations.
        /// </summary>
        /// <returns>A new <see cref="Result"/> instance.</returns>
        public static Result Succeeded() => new(true);

        /// <summary>
        /// Creates a new <see cref="Result"/> instance for failed operations.
        /// </summary>
        /// <param name="error">The error encountered during the operation.</param>
        /// <returns>A new <see cref="Result"/> instance.</returns>
        public static Result Failed(Exception error) => new(false, new[] { error });

        /// <summary>
        /// Creates a new <see cref="Result"/> instance for failed operations.
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <returns>A new <see cref="Result"/> instance.</returns>
        public static Result Failed(IEnumerable<Exception> errors) => new(false, errors.ToArray());

        /// <summary>
        /// Creates a new <see cref="Result"/> instance based on the presence of any errors.
        /// </summary>
        /// <param name="errors">The collection of errors.</param>
        /// <returns>A new <see cref="Result"/> instance - a success for zero errors or a failure if one or more errors.</returns>
        public static Result FromErrors(IEnumerable<Exception> errors) => errors.Any() ? Failed(errors) : Succeeded();

        private void Validate()
        {
            // Validates the arguments used to create the result. This is meant to ensure consistency and set 
            // expectations for success/failure states.

            // Rules:
            // - When Success is true, Errors is empty
            // - When Success is false, Errors is not empty

            if (Success)
            {
                if (Errors?.Any() is true)
                    throw new ArgumentException($"{nameof(Errors)} must be empty if {nameof(Success)} is true.");
            }
            else
            {
                if (Errors?.Any() is not true)
                    throw new ArgumentException($"{nameof(Errors)} must not be empty if {nameof(Success)} is false.");
            }
        }
    }

    /// <summary>
    /// Class representing the result of an operation.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    internal record Result<T> : Result, IResult<T>
        where T : class
    {
        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Value))]
        public override bool Success => base.Success; //We have to override the property here in order to use the MemberNotNullWhen attribute below

        /// <summary>
        /// Gets the result of the operation when successful or null when the operation is not successful.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="value">The result of the operation.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected Result(bool success, T? value, params Exception[] errors)
            : this(success, value, (IEnumerable<Exception>)errors)
        { }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="success">True if the operation is successful, false otherwise.</param>
        /// <param name="value">The result of the operation.</param>
        /// <param name="errors">The errors encountered during the operation, if any.</param>
        protected Result(bool success, T? value, IEnumerable<Exception> errors)
            : base(success, errors)
        {
            Value = value;

            Validate();
        }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance for successful operations.
        /// </summary>
        /// <param name="value">The result of the operation.</param>
        /// <returns>A new <see cref="Result{T}"/> instance.</returns>
        public static Result<T> Succeeded(T value) => new(true, value);

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance for failed operations.
        /// </summary>
        /// <param name="error">The error encountered during the operation.</param>
        /// <returns>A new <see cref="Result{T}"/> instance.</returns>
        public static new Result<T> Failed(Exception error) => Failed(new[] { error });

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance for failed operations.
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <returns>A new <see cref="Result{T}"/> instance.</returns>
        public static new Result<T> Failed(IEnumerable<Exception> errors) => new(false, null, errors);

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="result">The result value used to create the <typeparamref name="T"/> value.</param>
        /// <param name="value">The result of the operation.</param>
        /// <returns>A new <see cref="Result{T}"/> instance</returns>
        public static Result<T> Create(IResult result, T? value) => new(result.Success, value, result.Errors);

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <param name="resultBuilder">The result builder used to create the <typeparamref name="T"/> value.</param>
        /// <param name="value">The result of the operation.</param>
        /// <returns>A new <see cref="Result{T}"/> instance</returns>
        public static Result<T> Create(ResultBuilder resultBuilder, T? value)
        {
            var result = resultBuilder.Build();

            return new(result.Success, value, result.Errors);
        }

        /// <summary>
        /// Creates a new <see cref="Result{T}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type used to create the <typeparamref name="T"/> value.</typeparam>
        /// <param name="result">The result value used to create the <typeparamref name="T"/> value.</param>
        /// <param name="valueFactory">The method used to create the <typeparamref name="T"/> value.</param>
        /// <returns>A new <see cref="Result{T}"/> instance</returns>
        public static Result<T> Create<TResult>(TResult result, Func<TResult?, T> valueFactory) =>
            Create(result, valueFactory, Succeeded, Failed);

        /// <summary>
        /// Creates a new <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of <see cref="Result{T}"/> to create.</typeparam>
        /// <typeparam name="TValue">The type used to create the <typeparamref name="T"/> value.</typeparam>
        /// <param name="value">The result value used to create the <typeparamref name="T"/> value.</param>
        /// <param name="valueFactory">The method used to create the <typeparamref name="T"/> value.</param>
        /// <param name="successFactory">The method used to create a successful result.</param>
        /// <param name="failureFactory">The method used to create a failure result.</param>
        /// <returns>A new <typeparamref name="TResult"/> instance</returns>
        public static TResult Create<TResult, TValue>(
            TValue value,
            Func<TValue?, T> valueFactory,
            Func<T, TResult> successFactory,
            Func<IEnumerable<Exception>, TResult> failureFactory)
            where TResult : IResult<T>
        {
            T? resultValue;

            try
            {
                resultValue = valueFactory(value);
            }
            catch (Exception ex)
            {
                return failureFactory(new[] { ex });
            }

            return successFactory(resultValue);
        }

        private void Validate()
        {
            // Validates the arguments used to create the result. This is meant to ensure consistency and set 
            // expectations for success/failure states.

            // Rules:
            // - When Success is true, Value is not null.
            // - When Success is false, Value can be null.

            if (Success)
            {
                if (Value is null)
                    throw new ArgumentException($"{nameof(Value)} must not be null if {nameof(Success)} is true.");
            }
        }
    }
}
