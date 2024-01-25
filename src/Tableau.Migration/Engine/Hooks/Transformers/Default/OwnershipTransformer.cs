using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the user for a content item owner.
    /// </summary>
    public class OwnershipTransformer<TContent> : IContentTransformer<TContent>
        where TContent : IWithOwner
    {
        private readonly IMappedUserTransformer _userTransformer;

        /// <summary>
        /// Creates a new <see cref="OwnershipTransformer{TContent}"/> object.
        /// </summary>
        /// <param name="userTransformer">The user transformer.</param>
        public OwnershipTransformer(IMappedUserTransformer userTransformer)
        {
            _userTransformer = userTransformer;
        }

        /// <inheritdoc/>
        public async Task<TContent?> ExecuteAsync(TContent ctx, CancellationToken cancel)
        {
            var mapped = await _userTransformer.ExecuteAsync(ctx.Owner, cancel).ConfigureAwait(false);

            if (mapped is not null)
                ctx.Owner = mapped;

            return ctx;
        }
    }
}
