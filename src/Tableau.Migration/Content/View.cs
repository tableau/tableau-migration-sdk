using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal sealed class View : ContentBase, IView
    {
        /// <inheritdoc />
        public IList<ITag> Tags { get; set; }

        public View(IViewReferenceType view, IContentReference project, string? workbookName)
        {
            Guard.AgainstNullEmptyOrWhiteSpace(workbookName, () => workbookName);

            Id = view.Id;
            Name = Name = Guard.AgainstNullEmptyOrWhiteSpace(view.Name, () => view.Name);
            ContentUrl = Guard.AgainstNull(view.ContentUrl, () => view.ContentUrl);
            Location = project.Location.Append(workbookName).Append(Name);
            Tags = view.Tags.ToTagList(t => new Tag(t));
        }
    }
}
