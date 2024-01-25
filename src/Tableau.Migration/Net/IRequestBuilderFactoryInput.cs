using System;
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that contains the input given for a <see cref="IRequestBuilderFactory"/>, 
    /// used to bootstrap request building dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IRequestBuilderFactory"/> object, 
    /// this interface is only intended to be used to build <see cref="IRequestBuilderFactory"/> object.
    /// </remarks>
    public interface IRequestBuilderFactoryInput
    {
        /// <summary>
        /// Gets whether the input has been initialized.
        /// </summary>
        [MemberNotNullWhen(true, nameof(ServerUri))]
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the server URI to initialize the <see cref="IRequestBuilderFactory"/> with.
        /// </summary>
        Uri? ServerUri { get; }
    }
}
