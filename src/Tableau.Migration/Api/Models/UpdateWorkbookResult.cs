using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal sealed class UpdateWorkbookResult : IUpdateWorkbookResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Name { get; }
        
        /// <inheritdoc/>
        public string? Description { get; }

        /// <inheritdoc/>
        public string? ContentUrl { get; }

        /// <inheritdoc/>
        public bool ShowTabs { get; }

        /// <inheritdoc/>
        public DateTime CreatedAtUtc { get; }

        /// <inheritdoc/>
        public DateTime UpdatedAtUtc { get; }

        /// <inheritdoc/>
        public bool EncryptExtracts { get; }

        /// <summary>
        /// Creates a new <see cref="UpdateWorkbookResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API response.</param>
        public UpdateWorkbookResult(UpdateWorkbookResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Name, () => response.Item.Name);
            Description = response.Item.Description;
            ContentUrl = response.Item.ContentUrl;
            ShowTabs = response.Item.ShowTabs;
            CreatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.CreatedAt, () => response.Item.CreatedAt).ParseFromIso8601();
            UpdatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.UpdatedAt, () => response.Item.UpdatedAt).ParseFromIso8601();
            EncryptExtracts = response.Item.EncryptExtracts;
        }
    }
}
