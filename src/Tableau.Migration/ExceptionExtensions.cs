using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Returns whether or not the exception represents a cancellation of some kind.
        /// </summary>
        /// <param name="exception">The exception to test.</param>
        /// <returns>True if the exception indicates a cancelation, otherwise false.</returns>
        public static bool IsCancellationException(this Exception exception)
        {
            //Test basic types.
            if (exception is OperationCanceledException || exception is TaskCanceledException)
            {
                return true;
            }
            else if (exception is AggregateException aggException) //Test nested task canceled exception.
            {
                return aggException.InnerExceptions.All(inner => inner.IsCancellationException());
            }

            return false;
        }
    }
}
