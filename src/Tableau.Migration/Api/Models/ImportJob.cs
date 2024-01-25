using System;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class ImportJob : IImportJob
    {
        public Guid Id { get; }
        public string Type { get; }
        public DateTime CreatedAtUtc { get; }
        public int ProgressPercentage { get; }

        /// <summary>
        /// Creates a new <see cref="ImportJob"/> instance.
        /// </summary>
        /// <param name="response">The REST API user import job response.</param>
        public ImportJob(ImportJobResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);

            Type = Guard.AgainstNull(response.Item.Type, () => response.Item.Type);

            CreatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.CreatedAt, () => response.Item.CreatedAt).ParseFromIso8601();

            ProgressPercentage = response.Item.Progress;
        }
    }
}
