using System;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Intreface for a <see cref="IWorkbook"/> that has been downloaded
    /// and has full information necessary for re-publishing.
    /// </summary>
    public interface IPublishableWorkbook : IWorkbook, IFileContent, IWithViews, IChildPermissionsContent, IConnectionsContent
    {
        /// <summary>
        /// Gets the ID of the user to generate thumbnails as.
        /// </summary>
        Guid? ThumbnailsUserId { get; set; }
    }
}
