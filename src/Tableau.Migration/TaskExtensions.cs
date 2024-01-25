using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="Task"/> and <see cref="Task{TResult}"/> objects.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Get the results synchronously, applying best practices.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="task">The task to get synchronous results from.</param>
        /// <returns>The task's result.</returns>
        public static TResult AwaitResult<TResult>(this Task<TResult> task) => task.ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
