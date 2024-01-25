using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    internal static class IContentFileHandleExtensions
    {
        internal static async Task CloseTableauFileEditorAsync(this IContentFileHandle contentFileHandle, CancellationToken cancel)
        { 
            await contentFileHandle.Store.CloseTableauFileEditorAsync(contentFileHandle, cancel).ConfigureAwait(false);
        }
    }
}