using System;
using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item's label.
    /// </summary>
    public interface ILabel : IRestIdentifiable
    {
        /// <summary>
        /// Gets the site ID.
        /// </summary>        
        Guid SiteId { get; }

        /// <summary>
        /// Gets the owner ID.
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// Gets the user display name.
        /// </summary>
        string? UserDisplayName { get; }

        /// <summary>
        /// Gets the ID for the label's content item.
        /// </summary>
        Guid ContentId { get; }

        /// <summary>
        /// Gets the type for the label's content item.
        /// </summary>
        string? ContentType { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        string? Message { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        string? Value { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        string? Category { get; }

        /// <summary>
        /// Gets the active flag.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets the active flag.
        /// </summary>
        bool Elevated { get; }

        /// <summary>
        /// Gets the create timestamp.
        /// </summary>        
        string? CreatedAt { get; }

        /// <summary>
        /// Gets the update timestamp.
        /// </summary>
        string? UpdatedAt { get; }
    }
}
