using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Default <see cref="IAuthenticationTypeDomainMapping"/> implementation.
    /// </summary>
    public class AuthenticationTypeDomainMapping
        : AuthenticationTypeDomainMappingBase
    {
        private readonly IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions> _optionsProvider;

        /// <summary>
        /// Creates a new <see cref="AuthenticationTypeDomainMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        public AuthenticationTypeDomainMapping(IMigrationPlanOptionsProvider<AuthenticationTypeDomainMappingOptions> optionsProvider)
        {
            _optionsProvider = optionsProvider;
        }

        /// <summary>
        /// Executes the mapping for a user or group.
        /// </summary>
        /// <typeparam name="T">The <see cref="IUsernameContent"/> type.</typeparam>
        /// <param name="context">The mapping context.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The mapped context.</returns>
        protected override Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
        {
            var options = _optionsProvider.Get();
            var domain = context.ContentItem is IGroup ? options.GroupDomain : options.UserDomain;

            var mappedUsername = ContentLocation.ForUsername(domain, context.MappedLocation.Name);
            return context.MapTo(mappedUsername).ToTask();
        }
    }
}
