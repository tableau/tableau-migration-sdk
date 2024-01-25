using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class StatusNote : IStatusNote
    {
        /// <inheritdoc />
        public string? Type { get; }

        /// <inheritdoc />
        public string? Value { get; }

        /// <inheritdoc />
        public string? Text { get; }

        public StatusNote(JobResponse.JobType.StatusNoteType response)
        {
            Type = response.Type;
            Value = response.Value;
            Text = response.Text;
        }
    }
}
