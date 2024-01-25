using System;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal sealed class UpdateProjectResult : IUpdateProjectResult
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public Guid? ParentProjectId { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public string? ContentPermissions { get; }

        /// <inheritdoc />
        public Guid? ControllingPermissionsProjectId { get; }

        /// <inheritdoc />
        public Guid OwnerId { get; }

        /// <summary>
        /// Creates a new <see cref="UpdateProjectResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API response.</param>
        public UpdateProjectResult(UpdateProjectResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);
            ParentProjectId = response.Item.GetParentProjectId();
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Name, () => response.Item.Name);
            Description = response.Item.Description;
            ContentPermissions = response.Item.ContentPermissions;
            ControllingPermissionsProjectId = response.Item.GetControllingPermissionsProjectId();

            Guard.AgainstNull(response.Item.Owner, () => response.Item.Owner);
            OwnerId = Guard.AgainstDefaultValue(response.Item.Owner.Id, () => response.Item.Owner.Id);
        }
    }
}
