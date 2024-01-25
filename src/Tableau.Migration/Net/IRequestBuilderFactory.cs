namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for <see cref="IRequestBuilder"/> factories.
    /// </summary>
    public interface IRequestBuilderFactory
    {
        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> instance.
        /// </summary>
        /// <param name="path">The URI path.</param>
        /// <returns>A new <see cref="IRequestBuilder"/> instance.</returns>
        IRequestBuilder CreateUri(string path);
    }

    /// <summary>
    /// Interface for <see cref="IRequestBuilder"/> factories.
    /// </summary>
    public interface IRequestBuilderFactory<TRequestBuilder> : IRequestBuilderFactory
        where TRequestBuilder : IRequestBuilder
    {
        /// <summary>
        /// Creates a new <typeparamref name="TRequestBuilder"/> instance.
        /// </summary>
        /// <param name="path">The URI path.</param>
        /// <returns>A new <typeparamref name="TRequestBuilder"/> instance.</returns>
        new TRequestBuilder CreateUri(string path);
    }
}
