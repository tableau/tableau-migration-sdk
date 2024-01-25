namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client job status note model.
    /// </summary>
    public interface IStatusNote
    {
        /// <summary>
        /// Gets the status note's type.
        /// </summary>
        string? Type { get; }

        /// <summary>
        /// Gets the status note's value.
        /// </summary>
        string? Value { get; }

        /// <summary>
        /// Gets the status note's text.
        /// </summary>
        string? Text { get; }
    }
}
