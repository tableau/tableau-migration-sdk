using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class Workbook : ContainerContentBase, IWorkbook
    {
        public Workbook(IWorkbookType response, IContentReference project, IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            ContentUrl = Guard.AgainstNullEmptyOrWhiteSpace(response.ContentUrl, () => response.ContentUrl);

            ShowTabs = response.ShowTabs;
            Size = response.Size;
            WebpageUrl = response.WebpageUrl;
            EncryptExtracts = response.EncryptExtracts;

            Description = response.Description ?? string.Empty;
            CreatedAt = response.CreatedAt ?? string.Empty;
            UpdatedAt = response.UpdatedAt ?? string.Empty;
            WebpageUrl = response.WebpageUrl ?? string.Empty;

            Owner = owner;
            Tags = response.Tags.ToTagList(t => new Tag(t));

            Location = project.Location.Append(Name);
        }

        /// <inheritdoc />
        public bool ShowTabs { get; set; }

        /// <inheritdoc />
        public long Size { get; }

        /// <inheritdoc />
        public string? WebpageUrl { get; }

        /// <inheritdoc />
        public string CreatedAt { get; }

        /// <inheritdoc />
        public string? UpdatedAt { get; }

        /// <inheritdoc />
        public bool EncryptExtracts { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IList<ITag> Tags { get; set; }

        /// <inheritdoc />
        public IContentReference Owner { get; set; }
    }
}
