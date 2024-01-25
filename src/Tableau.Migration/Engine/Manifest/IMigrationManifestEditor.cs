using System;
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Manifest
{
    /// <summary>
    /// <see cref="IMigrationManifest"/> that can be edited.
    /// Used while a migration is in progress.
    /// </summary>
    public interface IMigrationManifestEditor : IMigrationManifest
    {
        /// <summary>
        /// Gets an object to edit manifest entries with.
        /// </summary>
        new IMigrationManifestEntryCollectionEditor Entries { get; }

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item from multiple <see cref="IResult"/> objects.
        /// </summary>
        /// <param name="results">The results to add errors from.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        public IMigrationManifestEditor AddErrors(IEnumerable<IResult> results)
        {
            foreach (var result in results)
            {
                AddErrors(result);
            }

            return this;
        }

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item from a <see cref="IResult"/> object.
        /// </summary>
        /// <param name="result">The result to add errors from.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        public IMigrationManifestEditor AddErrors(IResult result) => AddErrors(result.Errors);

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item.
        /// </summary>
        /// <param name="errors">The errors to add.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        IMigrationManifestEditor AddErrors(IEnumerable<Exception> errors);

        /// <summary>
        /// Adds top-level errors that are not related to any Tableau content item.
        /// </summary>
        /// <param name="errors">The errors to add.</param>
        /// <returns>This manifest editor, for fluent API usage.</returns>
        IMigrationManifestEditor AddErrors(params Exception[] errors);
    }
}
