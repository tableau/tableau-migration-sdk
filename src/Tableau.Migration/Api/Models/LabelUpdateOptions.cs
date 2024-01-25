using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Implementation for a content item's label update options.
    /// </summary>
    public class LabelUpdateOptions : ILabelUpdateOptions
    {
        /// <summary>
        /// Constructor to build from <see cref="ILabel"/>
        /// </summary>
        /// <param name="label"></param>
        public LabelUpdateOptions(ILabel label)
        {
            Value = label.Value;
            Message = label.Message;
            Active = label.Active;
            Elevated = label.Elevated;
        }

        /// <inheritdoc/>
        public string? Value { get; set; }

        /// <inheritdoc/>
        public string? Message { get; set; }

        /// <inheritdoc/>
        public bool Active { get; set; }

        /// <inheritdoc/>
        public bool Elevated { get; set; }
    }
}
