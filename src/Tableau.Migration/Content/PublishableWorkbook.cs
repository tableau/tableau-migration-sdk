using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Content
{
    internal sealed class PublishableWorkbook : ViewsWorkbook, IPublishableWorkbook
    {
        /// <inheritdoc />
        public Guid? ThumbnailsUserId { get; set; }

        /// <inheritdoc />
        public IContentFileHandle File { get; set; }

        /// <inheritdoc />
        public IImmutableList<IConnection> Connections { get; }

        public PublishableWorkbook(WorkbookResponse response, IContentReference project, IContentReference owner, IImmutableList<IConnection> connections, IImmutableList<IView> views, IContentFileHandle file)
            : base(Guard.AgainstNull(response.Item, () => response.Item), project, owner, views)
        {
            ThumbnailsUserId = null;
            Connections = connections;
            File = file;
        }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await File.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
