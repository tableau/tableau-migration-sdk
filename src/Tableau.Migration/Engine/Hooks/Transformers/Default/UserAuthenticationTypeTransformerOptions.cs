using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Options for <see cref="UserAuthenticationTypeTransformer"/>.
    /// </summary>
    public class UserAuthenticationTypeTransformerOptions
    {
        /// <summary>
        /// Gets the authentication type to set, defaults to <see cref="AuthenticationTypes.ServerDefault"/>.
        /// </summary>
        public string AuthenticationType { get; init; } = AuthenticationTypes.ServerDefault;
    }
}
