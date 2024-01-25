using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for classes that delay tasks.
    /// </summary>
    public interface ITaskDelayer
    {
        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <param name="cancel">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        Task DelayAsync(TimeSpan delay, CancellationToken cancel);
    }
}