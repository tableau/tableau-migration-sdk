using System;
using System.Collections.Generic;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// Default <see cref="IContentItemMigrationResult{TContent}"/> implementation.
    /// </summary>
    internal record ContentItemMigrationResult<TContent> : Result, IContentItemMigrationResult<TContent>
    {
        /// <inheritdoc />
        public bool ContinueBatch { get; }

        /// <inheritdoc />
        public bool IsCanceled { get; }

        /// <inheritdoc />
        public IMigrationManifestEntry ManifestEntry { get; }

        protected ContentItemMigrationResult(bool success, bool continueBatch, IMigrationManifestEntry manifestEntry, bool isCanceled, IEnumerable<Exception> errors)
            : base(success, errors)
        {
            ContinueBatch = continueBatch;
            ManifestEntry = manifestEntry;
            IsCanceled = isCanceled;
        }

        protected ContentItemMigrationResult(IResult baseResult, bool continueBatch, IMigrationManifestEntry manifestEntry, bool isCanceled)
            : this(baseResult.Success, continueBatch, manifestEntry, isCanceled, baseResult.Errors)
        { }

        protected ContentItemMigrationResult(bool success, bool continueBatch, IMigrationManifestEntry manifestEntry, bool isCanceled, params Exception[] errors)
            : this(success, continueBatch, manifestEntry, isCanceled, (IEnumerable<Exception>)errors)
        { }

        /// <summary>
        /// Creates a new <see cref="ContentItemMigrationResult{TContent}"/> instance for successful operations.
        /// </summary>
        /// <param name="manifestEntry">The migration manifest entry.</param>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>A new <see cref="ContentItemMigrationResult{TContent}"/> instance.</returns>
        public static ContentItemMigrationResult<TContent> Succeeded(IMigrationManifestEntry manifestEntry, bool continueBatch = true)
            => new(true, continueBatch, manifestEntry, false);

        /// <summary>
        /// Creates a new <see cref="ContentItemMigrationResult{TContent}"/> instance for failed operations.
        /// </summary>
        /// <param name="manifestEntry">The migration manifest entry.</param>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>A new <see cref="ContentItemMigrationResult{TContent}"/> instance.</returns>
        public static ContentItemMigrationResult<TContent> Failed(IMigrationManifestEntry manifestEntry, IEnumerable<Exception> errors, bool continueBatch = true)
            => new(false, continueBatch, manifestEntry, false, errors);

        /// <summary>
        /// Creates a new <see cref="ContentItemMigrationResult{TContent}"/> instance.
        /// </summary>
        /// <param name="result">The result to copy success/errors from.</param>
        /// <param name="manifestEntry">The migration manifest entry.</param>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>A new <see cref="ContentItemMigrationResult{TContent}"/> instance.</returns>
        public static ContentItemMigrationResult<TContent> FromResult(IResult result, IMigrationManifestEntry manifestEntry, bool continueBatch = true)
            => new(result.Success, continueBatch, manifestEntry, false, result.Errors);

        /// <summary>
        /// Creates a new <see cref="ContentItemMigrationResult{TContent}"/> instance for canceled operations.
        /// </summary>
        /// <param name="manifestEntry">The migration manifest entry.</param>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <param name="continueBatch">Whether or not the current migration batch should continue.</param>
        /// <returns>A new <see cref="ContentItemMigrationResult{TContent}"/> instance.</returns>
        public static ContentItemMigrationResult<TContent> Canceled(IMigrationManifestEntry manifestEntry, IEnumerable<Exception> errors, bool continueBatch = true)
            => new(false, continueBatch, manifestEntry, true, errors);

        /// <inheritdoc />
        public IContentItemMigrationResult<TContent> ForContinueBatch(bool continueBatch)
            => new ContentItemMigrationResult<TContent>(Success, continueBatch, ManifestEntry, IsCanceled, Errors);
    }
}
