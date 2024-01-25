using System;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IRequestBuilderFactoryInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IRequestBuilderFactoryInput"/> object, 
    /// which in turn is used to build an <see cref="IRequestBuilderFactory"/> object.
    /// End users are intended to inject the final <see cref="IRequestBuilderFactoryInput"/> result and not bootstrap objects.
    /// </remarks>
    public interface IRequestBuilderFactoryInputInitializer : IRequestBuilderFactoryInput
    {
        /// <summary>
        /// Initializes the <see cref="IRequestBuilderFactoryInputInitializer"/> object.
        /// </summary>
        /// <param name="serverUri">The server URI to initialize the <see cref="IRequestBuilderFactory"/> with.</param>
        void Initialize(Uri serverUri);
    }
}
