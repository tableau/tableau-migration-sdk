using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Publishing
{
    /// <summary>
    /// Interface for workbook publisher classes.
    /// </summary>
    public interface IWorkbookPublisher : IFilePublisher<IPublishWorkbookOptions, IResultWorkbook>
    { }
}
