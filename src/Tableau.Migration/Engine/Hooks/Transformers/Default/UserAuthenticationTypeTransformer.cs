using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that provides an authentication type for users, 
    /// defaulting to <see cref="AuthenticationTypes.ServerDefault"/>.
    /// See <see href="https://help.tableau.com/current/blueprint/en-gb/bp_administrative_roles_responsibilities.htm">Tableau API Reference</see> for details.
    /// </summary>
    public class UserAuthenticationTypeTransformer : ContentTransformerBase<IUser>
    {
        private readonly string _authenticationType;

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationTypeTransformer"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        public UserAuthenticationTypeTransformer(IMigrationPlanOptionsProvider<UserAuthenticationTypeTransformerOptions> optionsProvider)
        {
            _authenticationType = optionsProvider.Get().AuthenticationType;
        }

        /// <inheritdoc />
        public override Task<IUser?> ExecuteAsync(IUser itemToTransform, CancellationToken cancel)
        {
            itemToTransform.AuthenticationType = _authenticationType;
            return Task.FromResult<IUser?>(itemToTransform);
        }
    }
}
