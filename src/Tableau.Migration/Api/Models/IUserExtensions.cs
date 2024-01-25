using System.Text;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Extension methods for <see cref="IUser"/>.
    /// </summary>
    internal static class IUserExtensions
    {
        /// <summary>
        /// Converts the User object into a comma separated string to be used with the user_import call.
        /// The order of elements is important. 
        /// See https://help.tableau.com/current/server/en-us/csvguidelines.htm for guidelines.
        /// </summary>
        internal static void AppendCsvLine(this IUser user, StringBuilder builder)
        {
            var siteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => user.SiteRole);
            builder.AppendCsvLine(
                Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => user.Name),
                string.Empty, //Password (is intentionally empty)
                user.FullName,
                user.LicenseLevel,
                user.AdministratorLevel,
                user.CanPublish.ToString(),
                user.Email);
        }
    }
}
