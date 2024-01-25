using System;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Default <see cref="IRequestBuilderFactoryInput"/> and <see cref="IRequestBuilderFactoryInputInitializer"/> implementation.
    /// </summary>
    internal class RequestBuilderFactoryInput : IRequestBuilderFactoryInput, IRequestBuilderFactoryInputInitializer
    {
        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(ServerUri))]
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public Uri? ServerUri { get; private set; }

        /// <inheritdoc/>
        public void Initialize(Uri serverUri)
        {
            ServerUri = serverUri;

            IsInitialized = true;
        }
    }
}
