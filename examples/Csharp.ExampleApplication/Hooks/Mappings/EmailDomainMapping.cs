using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;

#region namespace

namespace MyMigrationApplication.Hooks.Mappings
{
    /// <summary>
    /// Mapping that appends an email domain to a username.
    /// </summary>
    public class EmailDomainMapping :
        ContentMappingBase<IUser>, // Base class to build mappings for content types
        ITableauCloudUsernameMapping
    {
        private readonly string _domain;

        /// <summary>
        /// Creates a new <see cref="EmailDomainMapping"/> object.
        /// </summary>
        /// <param name="optionsProvider">The options for this Mapping.</param>
        public EmailDomainMapping(
            IMigrationPlanOptionsProvider<EmailDomainMappingOptions> optionsProvider)
        {
            _domain = optionsProvider.Get().EmailDomain;
        }


        /// <summary>
        /// Creates a new <see cref="EmailDomainMapping"/> object.
        /// </summary>
        /// <param name="domain">THe email domain to use for the mapping.</param>
        public EmailDomainMapping(string domain)
        {
            _domain = domain;
        }

        /// <summary>
        /// Adds an email to the user if it doesn't exist.
        /// This is where the main logic of the mapping should reside.
        /// </summary>
        public override Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> userMappingContext, CancellationToken cancel)
        {
            var domain = userMappingContext.MappedLocation.Parent();
            // Re-use an existing email if it already exists.
            if (!string.IsNullOrEmpty(userMappingContext.ContentItem.Email))
                return userMappingContext.MapTo(domain.Append(userMappingContext.ContentItem.Email)).ToTask();

            // Takes the existing username and appends the default domain to build the email
            var testEmail = $"{userMappingContext.ContentItem.Name}@{_domain}";
            return userMappingContext.MapTo(domain.Append(testEmail)).ToTask();
        }
    }
}

#endregion