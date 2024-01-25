using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client job model.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets the job's type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the job's created timestamp.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the job's updated timestamp.
        /// </summary>
        DateTime? UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the job's completed timestamp.
        /// </summary>
        DateTime? CompletedAtUtc { get; }

        /// <summary>
        /// Gets the job's progress percentage.
        /// </summary>
        int ProgressPercentage { get; }

        /// <summary>
        /// Gets the job's finish code.
        /// </summary>
        int FinishCode { get; }

        /// <summary>
        /// Gets the job's status notes.
        /// </summary>
        IImmutableList<IStatusNote> StatusNotes { get; }
    }
}
