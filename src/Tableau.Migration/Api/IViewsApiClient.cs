using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that modifies workbook views
    /// </summary>
    public interface IViewsApiClient : IPermissionsContentApiClient, ITagsContentApiClient
    { }
}