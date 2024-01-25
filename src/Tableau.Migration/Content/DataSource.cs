﻿using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class DataSource : ContainerContentBase, IDataSource
    {
        public DataSource(IDataSourceType response, IContentReference project, IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            ContentUrl = Guard.AgainstNullEmptyOrWhiteSpace(response.ContentUrl, () => response.ContentUrl);

            Description = response.Description ?? string.Empty;
            CreatedAt = response.CreatedAt ?? string.Empty;
            UpdatedAt = response.UpdatedAt ?? string.Empty;

            EncryptExtracts = response.EncryptExtracts;
            HasExtracts = response.HasExtracts;
            IsCertified = response.IsCertified;
            UseRemoteQueryAgent = response.UseRemoteQueryAgent;

            WebpageUrl = response.WebpageUrl ?? string.Empty;

            Owner = owner;
            Tags = response.Tags.ToTagList(t => new Tag(t));

            Location = project.Location.Append(Name);
        }

        public DataSource(IDataSource item, IContentReference project, IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(item.Id, () => item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(item.Name, () => item.Name);
            ContentUrl = Guard.AgainstNullEmptyOrWhiteSpace(item.ContentUrl, () => item.ContentUrl);

            Description = item.Description ?? string.Empty;
            CreatedAt = item.CreatedAt ?? string.Empty;
            UpdatedAt = item.UpdatedAt ?? string.Empty;

            EncryptExtracts = item.EncryptExtracts;
            HasExtracts = item.HasExtracts;
            IsCertified = item.IsCertified;
            UseRemoteQueryAgent = item.UseRemoteQueryAgent;

            WebpageUrl = item.WebpageUrl ?? string.Empty;

            Owner = owner;
            Tags = item.Tags.ToTagList(t => new Tag(t));

            Location = project.Location.Append(Name);
        }

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string CreatedAt { get; }

        /// <inheritdoc/>
        public string? UpdatedAt { get; }

        /// <inheritdoc/>
        public bool EncryptExtracts { get; set; }

        /// <inheritdoc/>
        public bool HasExtracts { get; }

        /// <inheritdoc/>
        public bool IsCertified { get; }

        /// <inheritdoc/>
        public bool UseRemoteQueryAgent { get; set; }

        /// <inheritdoc/>
        public string? WebpageUrl { get; }

        // <inheritdoc/>
        public IContentReference Owner { get; set; }

        /// <inheritdoc/>
        public IList<ITag> Tags { get; set; }
    }
}
