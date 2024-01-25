using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that holds simulated workbook file data
    /// </summary>
    public class SimulatedWorkbookData : SimulatedDataWithConnections
    {
        /// <summary>
        /// Simulated Views data
        /// </summary>
        public List<SimulatedViewType> Views { get; set; } = new List<SimulatedViewType>();

        /// <summary>
        /// Simulated view type data
        /// </summary>
        public class SimulatedViewType
        {
            /// <summary>
            /// Default parameterless constructor
            /// </summary>
            public SimulatedViewType()
            { }

            /// <summary>
            /// Simulated view constructor
            /// </summary>
            /// <param name="view"></param>
            /// <param name="hidden"></param>
            public SimulatedViewType(ViewResponse.ViewType view, bool hidden)
            {
                View = view;
                Hidden = hidden;
            }

            /// <summary>
            /// Simulated View
            /// </summary>
            public ViewResponse.ViewType? View { get; set; }

            /// <summary>
            /// Simulated hidden flag
            /// </summary>
            public bool Hidden { get; set; } = false;
        }
    }
}
