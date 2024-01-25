using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Context for <see cref="BulkPostPublishContext{TPublish}"/> operations
    /// for published content items.
    /// </summary>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public class BulkPostPublishContext<TPublish>
    {
        /// <summary>
        /// Gets the content item being published.
        /// </summary>
        public IImmutableList<TPublish> PublishedItems { get; }

        /// <summary>
        /// Creates a new <see cref="BulkPostPublishContext{TPublish}"/> object.
        /// </summary>
        /// <param name="sourceItems">The source content items.</param>
        public BulkPostPublishContext(IEnumerable<TPublish> sourceItems)
        {
            PublishedItems = sourceItems.ToImmutableArray();
        }

        /// <summary>
        /// Creates a task that's successfully completed from the current context.
        /// </summary>
        /// <returns>The successfully completed task.</returns>
        public Task<BulkPostPublishContext<TPublish>?> ToTask()
            => Task.FromResult<BulkPostPublishContext<TPublish>?>(this);
    }
}
