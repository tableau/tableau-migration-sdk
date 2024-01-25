using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal sealed class Site : ISite
    {
        /// <summary>
        /// Gets the site content URL that is used by Tableau to represent the Default site on all installations.
        /// </summary>
        public const string DefaultContentUrl = "";

        /// <summary>
        /// Gets a <see cref="StringComparer"/> suitable for comparing content URLs for sites.
        /// </summary>
        public static readonly StringComparer ContentUrlComparer = StringComparer.OrdinalIgnoreCase;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string ContentUrl { get; }

        public Site(SiteResponse response)
        {
            var site = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(site.Id, () => response.Item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(site.Name, () => response.Item.Name);
            ContentUrl = Guard.AgainstNull(site.ContentUrl, () => response.Item.ContentUrl);
        }
    }
}
