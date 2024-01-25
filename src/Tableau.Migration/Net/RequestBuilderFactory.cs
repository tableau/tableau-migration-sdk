using System;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default <see cref="IRequestBuilder{TBuilder}"/> implementation.
    /// </summary>
    internal abstract class RequestBuilderFactory<TRequestBuilder> : IRequestBuilderFactory<TRequestBuilder>
        where TRequestBuilder : IRequestBuilder
    {
        private readonly Lazy<Uri> _baseUri;

        /// <summary>
        /// Gets the base URI for created URIs.
        /// </summary>
        protected Uri BaseUri => _baseUri.Value;

        public RequestBuilderFactory(IRequestBuilderFactoryInput input)
        {
            // Get the base URI lazily so input initialization can happen as late as possible.
            _baseUri = new(() => input.ServerUri ?? throw new InvalidOperationException($"{nameof(IRequestBuilderFactoryInput)} has not been initialized."));
        }

        /// <inheritdoc/>
        public abstract TRequestBuilder CreateUri(string path);

        /// <inheritdoc/>
        IRequestBuilder IRequestBuilderFactory.CreateUri(string path) => CreateUri(path);
    }
}
