using System;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Interface for an object that uses a REST API-style LUID identifier.
    /// </summary>
    public interface IRestIdentifiable
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid Id { get; }
    }
}
