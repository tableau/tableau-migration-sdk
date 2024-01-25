using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API view permissions methods.
    /// </summary>
    public sealed class ViewsRestApiSimulator : PermissionsRestApiSimulatorBase<WorkbookResponse.WorkbookType.ViewReferenceType>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulator"></param>
        public ViewsRestApiSimulator(TableauApiResponseSimulator simulator) :
            base(simulator, RestUrlPrefixes.Views, (data) => data.Views)
        {
        }
    }
}