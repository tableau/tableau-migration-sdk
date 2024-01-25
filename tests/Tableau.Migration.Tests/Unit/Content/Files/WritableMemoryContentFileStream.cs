using System;
using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    internal sealed class WritableMemoryContentFileStream : IContentFileStream
    {
        private readonly MemoryStream _memoryStream;
        private readonly Action<byte[]> _saveData;

        public Stream Content => _memoryStream;

        public WritableMemoryContentFileStream(Action<byte[]> saveData)
        {
            _memoryStream = new();
            _saveData = saveData;
        }

        public async ValueTask DisposeAsync()
        {
            _saveData(_memoryStream.ToArray());
            await _memoryStream.DisposeAsync();
        }
    }
}
