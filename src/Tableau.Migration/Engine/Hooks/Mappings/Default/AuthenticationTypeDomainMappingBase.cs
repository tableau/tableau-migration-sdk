using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Abstract base class for <see cref="IAuthenticationTypeDomainMapping"/> implementations.
    /// </summary>
    public abstract class AuthenticationTypeDomainMappingBase
        : IAuthenticationTypeDomainMapping
    {
        /// <summary>
        /// Executes the mapping for a user or group.
        /// </summary>
        /// <typeparam name="T">The <see cref="IUsernameContent"/> type.</typeparam>
        /// <param name="context">The mapping context.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The mapped context.</returns>
        protected abstract Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
            where T : IUsernameContent;

        /// <inheritdoc />
        public async Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
            => await ExecuteAsync<IUser>(ctx, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<ContentMappingContext<IGroup>?> ExecuteAsync(ContentMappingContext<IGroup> ctx, CancellationToken cancel)
            => await ExecuteAsync<IGroup>(ctx, cancel).ConfigureAwait(false);
    }
}
