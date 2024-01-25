using System.Collections.Generic;
namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that holds connections for simulated data.
    /// </summary>
    public class SimulatedDataWithConnections
    {
        /// <summary>
        /// Simulated Connection data
        /// </summary>
        public List<SimulatedConnection> Connections { get; set; } = new List<SimulatedConnection>();
    }
}