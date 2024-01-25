using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Publishing
{
    /// <summary>
    /// Interface for data source publisher classes.
    /// </summary>
    public interface IDataSourcePublisher : IFilePublisher<IPublishDataSourceOptions, IDataSource>
    { }
}
