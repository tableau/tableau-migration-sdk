using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Default <see cref="ITableauCloudUsernameMapping"/> implementation.
    /// </summary>
    public class TableauCloudUsernameMapping : ITableauCloudUsernameMapping
    {
        private readonly string _mailDomain;
        private readonly bool _useExistingEmail;

        /// <summary>
        /// Creates a new <see cref="TableauCloudUsernameMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        public TableauCloudUsernameMapping(IMigrationPlanOptionsProvider<TableauCloudUsernameMappingOptions> optionsProvider)
        {
            var opts = optionsProvider.Get();

            _mailDomain = opts.MailDomain;
            _useExistingEmail = opts.UseExistingEmail;
        }

        /// <inheritdoc />
        public Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            if (_useExistingEmail && !string.IsNullOrWhiteSpace(ctx.ContentItem.Email))
            {
                var emailLocation = ctx.MappedLocation.Parent().Append(ctx.ContentItem.Email);
                return ctx.MapTo(emailLocation).ToTask();
            }

            var generatedUsername = $"{ctx.MappedLocation.Name}@{_mailDomain}";
            var generatedLocation = ctx.MappedLocation.Parent().Append(generatedUsername);
            return ctx.MapTo(generatedLocation).ToTask();
        }
    }
}
