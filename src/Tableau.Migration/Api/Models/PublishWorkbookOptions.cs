using System;
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client workbook publish options. 
    /// </summary>
    public class PublishWorkbookOptions : IPublishWorkbookOptions
    {
        ///<inheritdoc/>
        public string Name { get; }

        ///<inheritdoc/>
        public string Description { get; }
        
        ///<inheritdoc/>
        public bool ShowTabs { get; }

        ///<inheritdoc/>
        public bool EncryptExtracts { get; }

        ///<inheritdoc/>
        public Guid? ThumbnailsUserId { get; }

        ///<inheritdoc/>
        public bool SkipConnectionCheck { get; } = true;

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
        /// Creates a new <see cref="PublishWorkbookOptions"/> instance.
        /// </summary>
        /// <param name="workbook">The publishable workbook information.</param>
        /// <param name="file">The workbook file as a <see cref="Stream"/></param>
        /// <param name="fileType">The type of workbook file.</param>
        public PublishWorkbookOptions(IPublishableWorkbook workbook, Stream file, string fileType = WorkbookFileTypes.Twbx)
        {
            Name = workbook.Name;
            Description = workbook.Description;
            ShowTabs = workbook.ShowTabs;
            EncryptExtracts = workbook.EncryptExtracts;
            ThumbnailsUserId = workbook.ThumbnailsUserId;
            ProjectId = ((IContainerContent)workbook).Container.Id;
            File = file;
            FileName = workbook.File.OriginalFileName;
            FileType = fileType;
        }
    }
}
