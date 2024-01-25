using System;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client import job model.
    /// </summary>
    public interface IImportJob
    {
        /// <summary>
        /// Gets the job's unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the job's type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the job's created timestamp.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the job's progress percentage.
        /// </summary>
        int ProgressPercentage { get; }
    }
}
