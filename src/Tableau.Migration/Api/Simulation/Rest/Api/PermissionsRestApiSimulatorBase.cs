using System;
using System.Collections.Generic;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Base class for objects that define simulation of Tableau REST API permissions methods.
    /// </summary>
    public abstract class PermissionsRestApiSimulatorBase<TContent>
        where TContent : IRestIdentifiable, INamedContent
    {
        /// <summary>
        /// Gets the URL prefix for the content permissions.
        /// </summary>
        protected readonly string ContentTypeUrlPrefix;

        /// <summary>
        /// Gets the simulated permission create API method.
        /// </summary>
        public MethodSimulator CreatePermissions { get; }

        /// <summary>
        /// Gets the simulated permission query API method.
        /// </summary>
        public MethodSimulator QueryPermissions { get; }

        /// <summary>
        /// Creates a new <see cref="PermissionsRestApiSimulatorBase{TContent}"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        /// <param name="contentTypeUrlPrefix">The content type's URl prefix.</param>
        /// <param name="getContent">Delegate used to retrieve content items by ID.</param>
        public PermissionsRestApiSimulatorBase(
            TableauApiResponseSimulator simulator,
            string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent)
        {
            ContentTypeUrlPrefix = contentTypeUrlPrefix;

            CreatePermissions = simulator.SetupRestPut(
                SiteEntityUrl(ContentTypeUrlPrefix, "permissions"),
                new RestPermissionsCreateResponseBuilder<TContent>(simulator.Data, simulator.Serializer, ContentTypeUrlPrefix, getContent));

            QueryPermissions = simulator.SetupRestGet(
                SiteEntityUrl(ContentTypeUrlPrefix, "permissions"),
                new RestPermissionsGetResponseBuilder<TContent>(simulator.Data, simulator.Serializer, ContentTypeUrlPrefix, getContent));
        }
    }
}
