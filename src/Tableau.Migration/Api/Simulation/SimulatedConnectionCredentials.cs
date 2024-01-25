using Tableau.Migration.Api.Rest.Models.Requests;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// The simulated connection credentials class.
    /// </summary>
    public class SimulatedConnectionCredentials
    {
        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        public SimulatedConnectionCredentials()
        { }

        /// <summary>
        /// Constructor to build from 
        /// <see cref="CommitWorkbookPublishRequest.WorkbookType.ConnectionType.ConnectionCredentialsType"/>
        /// </summary>
        /// <param name="response">The credentials type from workbook publish request.</param>
        public SimulatedConnectionCredentials(
            CommitWorkbookPublishRequest.WorkbookType.ConnectionType.ConnectionCredentialsType? response)
        {
            if (response == null)
            {
                return;
            }

            Name = response.Name;
            Password = response.Password;
            Embed = response.Embed;
            OAuth = response.OAuth;
        }

        /// <summary>
        /// Gets or sets the connection credentials name for the request's project.
        /// </summary>            
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the connection credentials password for the request's project.
        /// </summary>            
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the connection credentials embed flag for the request's project.
        /// </summary>            
        public string? Embed { get; set; }

        /// <summary>
        /// Gets or sets the connection credentials embed flag for the request's project.
        /// </summary>            
        public string? OAuth { get; set; }
    }
}
