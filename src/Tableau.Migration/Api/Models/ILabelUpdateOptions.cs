namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for a content item's label update options.
    /// </summary>
    public interface ILabelUpdateOptions
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Gets or sets the active flag.
        /// </summary>
        public bool Active { get; }

        /// <summary>
        /// Gets or sets the active flag.
        /// </summary>
        public bool Elevated { get; }
    }
}
