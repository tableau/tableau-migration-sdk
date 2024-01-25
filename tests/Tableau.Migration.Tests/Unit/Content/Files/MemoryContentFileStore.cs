using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class MemoryContentFileStore : IContentFileStore
    {
        private readonly ConcurrentDictionary<string, byte[]> _fileData = new();
        private readonly ConcurrentDictionary<string, ITableauFileEditor> _editors = new();

        public bool HasOpenTableauFileEditor => _editors.Any();

        public byte[] GetFileData(string path) => _fileData[path];

        public virtual IContentFileHandle Create(string relativePath, string originalFileName)
        {
            _fileData.GetOrAdd(relativePath, Array.Empty<byte>());
            return new ContentFileHandle(this, relativePath, originalFileName);
        }

        public virtual IContentFileHandle Create<TContent>(TContent contentItem, string originalFileName)
            => Create(Guid.NewGuid().ToString(), originalFileName);

        public virtual Task DeleteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            _fileData.TryRemove(handle.Path, out var _);
            return Task.CompletedTask;
        }

        public virtual ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        public virtual Task<IContentFileStream> OpenReadAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            IContentFileStream stream = new ContentFileStream(new MemoryStream(_fileData[handle.Path]));
            return Task.FromResult(stream);
        }

        public virtual Task<IContentFileStream> OpenWriteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            IContentFileStream writableStream = new WritableMemoryContentFileStream(data =>
            {
                _fileData.AddOrUpdate(handle.Path, data, (k, v) => data);
            });
            return Task.FromResult(writableStream);
        }

        public virtual async Task<ITableauFileEditor> GetTableauFileEditorAsync(IContentFileHandle handle,
            CancellationToken cancel, bool? zipFormatOverride = null)
        {
            return await Task.Run(() =>
            {
                return _editors.GetOrAdd(handle.Path,
                    (path) => TableauFileEditor.OpenAsync(handle, cancel, zipFormatOverride).GetAwaiter().GetResult());
            });
        }

        public virtual async Task CloseTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            if (_editors.TryRemove(handle.Path, out var editor))
            {
                await editor.DisposeAsync();
            }
        }
    }
}
