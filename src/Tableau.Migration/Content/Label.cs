using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    ///<inheritdoc/>
    internal class Label : ILabel
    {
        public Label(LabelsResponse.LabelType response)
        {
            Guard.AgainstNull(response, () => nameof(response));
            Id = Guard.AgainstDefaultValue(response.Id, () => nameof(response.Id));

            var site = Guard.AgainstNull(response.Site, () => nameof(response.Site));
            SiteId = Guard.AgainstDefaultValue(site.Id, () => nameof(response.Site.Id));

            var owner = Guard.AgainstNull(response.Owner, () => nameof(response.Owner));
            SiteId = Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));

            UserDisplayName = Guard.AgainstNull(response.UserDisplayName, () => nameof(response.UserDisplayName));

            ContentId = Guard.AgainstDefaultValue(response.ContentId, () => nameof(response.ContentId));

            ContentType = Guard.AgainstNull(response.ContentType, () => nameof(response.ContentType));
            Message = response.Message;
            Value = Guard.AgainstNull(response.Value, () => nameof(response.Value));
            Category = Guard.AgainstNull(response.Category, () => nameof(response.Category));
            Active = response.Active;
            Elevated = response.Elevated;
            CreatedAt = response.CreatedAt;
            UpdatedAt = response.UpdatedAt;
        }

        ///<inheritdoc/>
        public Guid Id { get; set; }

        ///<inheritdoc/>
        public Guid SiteId { get; set; }

        ///<inheritdoc/>
        public Guid OwnerId { get; set; }

        ///<inheritdoc/>
        public string UserDisplayName { get; set; }

        ///<inheritdoc/>
        public Guid ContentId { get; set; }

        ///<inheritdoc/>
        public string ContentType { get; set; }

        ///<inheritdoc/>
        public string? Message { get; set; }

        ///<inheritdoc/>
        public string Value { get; set; }

        ///<inheritdoc/>
        public string Category { get; set; }

        ///<inheritdoc/>
        public bool Active { get; set; }

        ///<inheritdoc/>
        public bool Elevated { get; set; }

        ///<inheritdoc/>
        public string? CreatedAt { get; set; }

        ///<inheritdoc/>
        public string? UpdatedAt { get; set; }
    }
}
