using System;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that can simulate API responses like Tableau Server/Cloud.
    /// Used in 'simulation' integration tests so real SDK components can be tested together without full Tableau Server/Cloud infrastructure.
    /// Unless overridden all responses are successful and well-formed.
    /// </summary>
    public sealed class TableauApiSimulator
    {
        /// <summary>
        /// Gets the URL for the simulated Tableau Server.
        /// </summary>
        public Uri ServerUrl { get; }

        /// <summary>
        /// Gets the simulator for mocking server responses.
        /// </summary>
        public TableauApiResponseSimulator ResponseSimulator { get; }

        /// <summary>
        /// Gets the data store for the API.
        /// </summary>
        public TableauData Data { get; }

        /// <summary>
        /// Gets the simulator for REST API requests.
        /// </summary>
        public RestApiSimulator RestApi { get; }

        /// <summary>
        /// Creates a new <see cref="TableauApiSimulator"/> object.
        /// </summary>
        /// <param name="serverUrl">The base server URL.</param>
        /// <param name="serializer">A serializer to use to produce responses.</param>
        /// <param name="defaultSignedInUser">Default signed in user to user. If none is provided, the simulated server will have no users.</param>
        /// <param name="defaultDomain">The default domain of the site.</param>
        public TableauApiSimulator(Uri serverUrl, IHttpContentSerializer serializer,
            UsersResponse.UserType? defaultSignedInUser = null, string defaultDomain = Constants.LocalDomain)
        {
            ServerUrl = serverUrl;
            Data = new(defaultSignedInUser, defaultDomain);
            ResponseSimulator = new TableauApiResponseSimulator(ServerUrl, Data, serializer);
            RestApi = new(ResponseSimulator);
        }
    }
}
