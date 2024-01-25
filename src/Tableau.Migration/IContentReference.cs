using System;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that describes information on how to reference an item of content,
    /// for example through a Tableau API.
    /// </summary>
    public interface IContentReference : IEquatable<IContentReference>
    {
        /// <summary>
        /// Gets the unique identifier of the content item, 
        /// corresponding to the LUID in the Tableau REST API.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Get the site-unique "content URL" of the content item, 
        /// or an empty string if the content type does not use a content URL.
        /// </summary>
        string ContentUrl { get; }

        /// <summary>
        /// Gets the logical location path of the content item,
        /// for project-level content this is the project path and the content item name.
        /// </summary>
        ContentLocation Location { get; }

        /// <summary>
        /// Gets the name of the content item.
        /// This is equivalent to the last segment of the <see cref="Location"/>.
        /// Renames should be performed through mapping.
        /// </summary>
        string Name { get; }
    }
}
