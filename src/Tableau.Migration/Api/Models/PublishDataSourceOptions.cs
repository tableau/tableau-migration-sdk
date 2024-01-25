using System;
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client data source publish options. 
    /// </summary>
    public class PublishDataSourceOptions : IPublishDataSourceOptions
    {
        ///<inheritdoc/>
        public string Name { get; }

        ///<inheritdoc/>
        public string Description { get; }

        ///<inheritdoc/>
        public bool UseRemoteQueryAgent { get; }

        ///<inheritdoc/>
        public bool EncryptExtracts { get; }

        ///<inheritdoc/>
        public bool Overwrite { get; } = true;

        ///<inheritdoc/>
        public Guid ProjectId { get; }

        ///<inheritdoc/>
        public Stream File { get; }

        ///<inheritdoc/>
        public string FileName { get; }

        ///<inheritdoc/>
        public string FileType { get; }

        /// <summary>
        /// Creates a new <see cref="PublishDataSourceOptions"/> instance.
        /// </summary>
        /// <param name="dataSource">The publishable data source information.</param>
        /// <param name="file">The data source file as a <see cref="Stream"/></param>
        /// <param name="fileType">The type of data source file.</param>
        public PublishDataSourceOptions(IPublishableDataSource dataSource, Stream file, string fileType = DataSourceFileTypes.Tdsx)
        {
            Name = dataSource.Name;
            Description = dataSource.Description;
            UseRemoteQueryAgent = dataSource.UseRemoteQueryAgent;
            EncryptExtracts = dataSource.EncryptExtracts;
            ProjectId = ((IContainerContent)dataSource).Container.Id;
            File = file;
            FileName = dataSource.File.OriginalFileName;
            FileType = fileType;
        }
    }
}
