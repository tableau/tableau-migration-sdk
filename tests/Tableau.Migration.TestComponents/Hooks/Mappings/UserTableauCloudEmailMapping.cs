using System.Net.Mail;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;

namespace Tableau.Migration.TestComponents.Hooks.Mappings
{
    /// <summary>
    /// Transformer that uses the base email plus a guid to create an email for every user.
    /// Example. sfroehlich@tableau.com would turn into sfroehlich+df334d533a96449e808e109d55e60cdd@tableau.com
    /// </summary>
    public class UserTableauCloudEmailMapping : ContentMappingBase<IUser>
    {
        private readonly MailAddress _baseEmail;

        /// <summary>
        /// Creates a new <see cref="UserTableauCloudEmailMapping"/> object.
        /// </summary>
        /// <param name="baseEmail">The options for this Transformer.</param>
        public UserTableauCloudEmailMapping(MailAddress baseEmail)
        {
            _baseEmail = baseEmail;
        }

        /// <summary>
        /// Adds an email to the user if it doesn't exist. 
        /// </summary>
        public override Task<ContentMappingContext<IUser>?> ExecuteAsync(ContentMappingContext<IUser> ctx, CancellationToken cancel)
        {
            var domain = ctx.MappedLocation.Parent();

            // Re-use an existing email if it already exists.
            if (!string.IsNullOrEmpty(ctx.ContentItem.Email))
                return ctx.MapTo(domain.Append(ctx.ContentItem.Email)).ToTask();

            // Takes the existing "Name" and appends the default domain to build the email
            var testEmail = $"{_baseEmail.User}+TEST{Guid.NewGuid():N}@{_baseEmail.Host}";
            return ctx.MapTo(domain.Append(testEmail)).ToTask();
        }
    }
}