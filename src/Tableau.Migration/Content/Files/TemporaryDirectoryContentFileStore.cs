using System.IO;
using System.IO.Abstractions;
using Tableau.Migration.Config;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFileStore"/> implementation that stores
    /// files in a random temporary directory.
    /// </summary>
    public class TemporaryDirectoryContentFileStore
        : DirectoryContentFileStore
    {
        /// <summary>
        /// Creates a new <see cref="TemporaryDirectoryContentFileStore"/> object.
        /// </summary>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="pathResolver">The path resolver to use.</param>
        /// <param name="configReader">A config reader to get the root path and other options from.</param>
        public TemporaryDirectoryContentFileStore(IFileSystem fileSystem, IContentFilePathResolver pathResolver, IConfigReader configReader)
            : base(fileSystem, pathResolver, configReader, Path.GetRandomFileName())
        { }
    }
}
