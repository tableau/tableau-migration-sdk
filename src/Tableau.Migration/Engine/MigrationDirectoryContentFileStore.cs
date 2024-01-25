using System.IO.Abstractions;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Represents a content file store that stores files in a per-migration sub directory.
    /// </summary>
    public class MigrationDirectoryContentFileStore : DirectoryContentFileStore
    {
        /// <summary>
        /// Creates a new <see cref="MigrationDirectoryContentFileStore"/> object.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="pathResolver">The path resolver.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="migrationInput">The migration input to get the migration ID from.</param>
        public MigrationDirectoryContentFileStore(IFileSystem fileSystem, IContentFilePathResolver pathResolver, IConfigReader configReader, IMigrationInput migrationInput)
            : base(fileSystem, pathResolver, configReader, $"migration-{migrationInput.MigrationId:N}")
        { }
    }
}
