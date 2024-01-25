using System;
using System.Collections.Generic;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Options for <see cref="DefaultPermissionsContentTypeOptions"/>.
    /// </summary>
    public class DefaultPermissionsContentTypeOptions
    {
        /// <summary>
        /// <para>
        /// Gets or sets the corresponding URL segments for default permissions content types.
        /// </para>
        /// <para>
        /// For example, for the URL "/api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks" the URL segment would be "workbooks".
        /// </para>
        /// </summary>
        public HashSet<string> UrlSegments { get; } = new(DefaultPermissionsContentTypeUrlSegments.GetAll(), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new <see cref="DefaultPermissionsContentTypeOptions"/> instance.
        /// </summary>
        /// <param name="urlSegments">The optional custom default permissions content types.</param>
        public DefaultPermissionsContentTypeOptions(IEnumerable<string>? urlSegments = null)
        {
            if (!urlSegments.IsNullOrEmpty())
            {
                foreach (var urlSegment in urlSegments)
                {
                    if (!String.IsNullOrWhiteSpace(urlSegment))
                        UrlSegments.Add(urlSegment);
                }
            }
        }
    }
}
