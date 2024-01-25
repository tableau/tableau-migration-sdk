using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface representing the result of an operation.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets whether the operation was successful.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Gets any exceptions encountered during the operation.
        /// </summary>
        IImmutableList<Exception> Errors { get; }

        /// <summary>
        /// Casts a failure result to another type.
        /// </summary>
        /// <typeparam name="U">The type to cast to.</typeparam>
        /// <returns>The casted result.</returns>
        /// <exception cref="InvalidOperationException">If the result is not a failure result.</exception>
        public IResult<U> CastFailure<U>()
            where U : class
        {
            if (Success)
                throw new InvalidOperationException("Cannot case a successful result without a value.");

            return Result<U>.Failed(Errors);
        }
    }

    /// <summary>
    /// Interface representing the result of an operation.
    /// </summary>
    /// <typeparam name="T">The result's value type</typeparam>
    public interface IResult<T> : IResult
        where T : class
    {
        /// <summary>
        /// Gets whether the operation was successful.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        new bool Success { get; } //We have to override the property here in order to use the MemberNotNullWhen attribute below

        /// <summary>
        /// Gets the result of the operation when successful or null when the operation is not successful.
        /// </summary>
        T? Value { get; }
    }
}
