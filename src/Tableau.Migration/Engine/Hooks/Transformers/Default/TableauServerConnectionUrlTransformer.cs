//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files.Xml;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Default XML transformer that updates Tableau Server connections
    /// to account for the URL components of the published data source possibly
    /// changing during migration.
    /// </summary>
    public class TableauServerConnectionUrlTransformer : XmlContentTransformerBase<IPublishableWorkbook>
    {
        #region - Constants/Fields -

        private const string CONNECTION_EL = "connection";
        private const string DATA_SOURCE_EL = "datasource";
        private const string REPOSITORY_LOCATION_EL = "repository-location";

        private const string CHANNEL_ATTR = "channel";
        private const string CLASS_ATTR = "class";
        private const string DATABASE_ATTR = "dbname";
        private const string ID_ATTR = "id";
        private const string PATH_ATTR = "path";
        private const string PORT_ATTR = "port";
        private const string SERVER_ATTR = "server";
        private const string SITE_ATTR = "site";

        internal const string TABLEAU_SERVER_CONNECTION_CLASS = "sqlproxy";

        private readonly ITableauApiEndpointConfiguration? _destinationConfig; //Null if the destination is not an API.
        private readonly IDestinationContentReferenceFinder<IDataSource> _mappedDataSourceFinder;
        private readonly ILogger<TableauServerConnectionUrlTransformer> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        private readonly HashSet<string> _contentUrlWarnings;

        #endregion

        #region - Ctor -

        /// <summary>
        /// Creates a new <see cref="TableauServerConnectionUrlTransformer"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="logger">A logger to use.</param>
        /// <param name="localizer">A localizer to user.</param>
        public TableauServerConnectionUrlTransformer(
            IMigration migration,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<TableauServerConnectionUrlTransformer> logger,
            ISharedResourcesLocalizer localizer)
        {
            _contentUrlWarnings = new(StringComparer.OrdinalIgnoreCase);
            _destinationConfig = migration.Plan.Destination as ITableauApiEndpointConfiguration;
            _mappedDataSourceFinder = destinationFinderFactory.ForDestinationContentType<IDataSource>();
            _logger = logger;
            _localizer = localizer;
        }

        #endregion

        #region - Private XML Helper Methods -

        private static string GetWarningKey(ContentLocation workbookLocation, string sourceContentUrl)
            => $"{workbookLocation}/{sourceContentUrl}";

        private async Task UpdateContentUrlAsync(IPublishableWorkbook workbook, XAttribute attr, CancellationToken cancel)
        {
            var sourceContentUrl = attr.Value;

            //Find the published data source reference.
            var destDataSourceRef = await _mappedDataSourceFinder.FindBySourceContentUrlAsync(sourceContentUrl, cancel)
                .ConfigureAwait(false);

            if (destDataSourceRef is not null)
            {
                attr.Value = destDataSourceRef.ContentUrl;
            }
            else
            {
                var warningKey = GetWarningKey(workbook.Location, sourceContentUrl);
                if (_contentUrlWarnings.Add(warningKey)) // false if already present.
                {
                    _logger.LogWarning(_localizer[SharedResourceKeys.PublishedDataSourceReferenceNotFoundLogMessage],
                        sourceContentUrl, workbook.Location);
                }
            }
        }

        private async Task UpdateConnectionAsync(IPublishableWorkbook workbook, XElement connectionElement, CancellationToken cancel)
        {
            //Only modify Tableau Server connections.
            var isServerConnection = connectionElement.GetFeatureFlaggedAttributes(CLASS_ATTR)
                .Any(a => a.Value is TABLEAU_SERVER_CONNECTION_CLASS);

            if (!isServerConnection)
            {
                return;
            }

            //Update connection details based on destination endpoint URL.
            if (_destinationConfig is not null)
            {
                var config = _destinationConfig.SiteConnectionConfiguration;

                foreach (var channelAttr in connectionElement.GetFeatureFlaggedAttributes(CHANNEL_ATTR))
                {
                    channelAttr.Value = config.ServerUrl.Scheme.ToLower();
                }

                foreach (var portAttr in connectionElement.GetFeatureFlaggedAttributes(PORT_ATTR))
                {
                    portAttr.Value = config.ServerUrl.Port.ToString();
                }

                foreach (var serverAttr in connectionElement.GetFeatureFlaggedAttributes(SERVER_ATTR))
                {
                    serverAttr.Value = config.ServerUrl.Host;
                }
            }

            //Update the content URL with FFS safety.
            foreach (var dbNameAttribute in connectionElement.GetFeatureFlaggedAttributes(DATABASE_ATTR))
            {
                await UpdateContentUrlAsync(workbook, dbNameAttribute, cancel)
                    .ConfigureAwait(false);
            }
        }

        private async Task UpdateRepositoryLocationAsync(IPublishableWorkbook workbook, XElement repoLocationElement, CancellationToken cancel)
        {
            //Only update data source repo locations, not the top workbook repo location.
            var isDataSourceRepoLocation = repoLocationElement.Parent is not null &&
                repoLocationElement.Parent.Name.MatchFeatureFlagName(DATA_SOURCE_EL);

            if (!isDataSourceRepoLocation)
            {
                return;
            }

            //Update repository location details based on destination endpoint URL.
            if (_destinationConfig is not null)
            {
                var siteId = _destinationConfig.SiteConnectionConfiguration.SiteContentUrl;

                foreach (var pathAttr in repoLocationElement.GetFeatureFlaggedAttributes(PATH_ATTR))
                {
                    pathAttr.Value = $"/t/{siteId}/datasources";
                }

                foreach (var siteAttr in repoLocationElement.GetFeatureFlaggedAttributes(SITE_ATTR))
                {
                    siteAttr.Value = siteId;
                }
            }

            //Update the content URL with FFS safety.
            foreach (var idAttribute in repoLocationElement.GetFeatureFlaggedAttributes(ID_ATTR))
            {
                await UpdateContentUrlAsync(workbook, idAttribute, cancel)
                    .ConfigureAwait(false);
            }
        }

        #endregion

        #region - XmlContentTransformerBase<IPublishableWorkbook> Implementation -

        /// <inheritdoc />
        protected override bool NeedsXmlTransforming(IPublishableWorkbook ctx)
            => ctx.Connections.Any(c => c.Type is TABLEAU_SERVER_CONNECTION_CLASS);

        /// <inheritdoc />
        public override async Task TransformAsync(IPublishableWorkbook ctx, XDocument xml, CancellationToken cancel)
        {
            //Update <connection> elements.
            foreach (var connectionElement in xml.GetFeatureFlaggedDescendants(CONNECTION_EL))
            {
                await UpdateConnectionAsync(ctx, connectionElement, cancel)
                    .ConfigureAwait(false);
            }

            //Update <repository-location /> elements that live at the  data source level.
            foreach (var repoLocationElement in xml.GetFeatureFlaggedDescendants(REPOSITORY_LOCATION_EL))
            {
                await UpdateRepositoryLocationAsync(ctx, repoLocationElement, cancel)
                    .ConfigureAwait(false);
            }
        }

        #endregion
    }
}
