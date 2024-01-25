using System;
using System.IO;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFilePathResolver"/> implementation that generates content item file store paths by their content type.
    /// </summary>
    public class ContentTypeFilePathResolver : IContentFilePathResolver
    {
        /// <inheritdoc />
        public string ResolveRelativePath<TContent>(TContent contentItem, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName) ?? string.Empty;

            if (contentItem is IDataSource ds)
            {
                return Path.Combine("data-sources", $"data-source-{ds.Id:N}{extension}");
            }
            else if (contentItem is IWorkbook wb)
            {
                return Path.Combine("workbooks", $"workbook-{wb.Id:N}{extension}");
            }

            throw new ArgumentException($"Cannot generate a file store path for content type {typeof(TContent).Name}");
        }
    }
}
