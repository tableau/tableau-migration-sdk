using System;

namespace Tableau.Migration.Net.Rest
{
    /// <summary>
    /// Interface for <see cref="IRestRequestBuilder"/> factories.
    /// </summary>
    public interface IRestRequestBuilderFactory : IRequestBuilderFactory<IRestRequestBuilder>
    {
        /// <summary>
        /// Sets the default API version to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="version">The API version.</param>
        void SetDefaultApiVersion(string? version);

        /// <summary>
        /// Sets the default site ID to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        void SetDefaultSiteId(Guid? siteId);

        /// <summary>
        /// Sets the default site ID to use when creating <see cref="IRestRequestBuilder"/> instances. 
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        void SetDefaultSiteId(string? siteId);
    }
}