using System;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface representing the disposable result of an operation.
    /// </summary>
    /// <typeparam name="T">The result's value type</typeparam>
    public interface IAsyncDisposableResult<T> : IResult<T>, IAsyncDisposable
        where T : class, IAsyncDisposable
    { }
}
