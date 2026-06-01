//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Default JSON transformer that updates server URL, site URL, and project LUID in flow JSON:
    /// output steps (serverUrl, projectLuid on node), input steps (server, siteUrlName in connectionAttributes).
    /// Replaces only when the current value matches the source (copyFlows-style).
    /// </summary>
    public class TableauServerConnectionUrlFlowTransformer : JsonContentTransformerBase<IPublishableFlow>
    {
        #region - Constants/Fields -

        private readonly ITableauApiEndpointConfiguration? _destinationConfig;
        private readonly ITableauApiEndpointConfiguration? _sourceConfig;
        private readonly IDestinationContentReferenceFinder<IProject> _projectFinder;
        private readonly ILogger<TableauServerConnectionUrlFlowTransformer> _logger;

        #endregion

        #region - Ctor -

        /// <summary>
        /// Creates a new <see cref="TableauServerConnectionUrlFlowTransformer"/> instance.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="destinationFinderFactory">Factory to create destination content finders.</param>
        /// <param name="logger">Logger.</param>
        public TableauServerConnectionUrlFlowTransformer(
            IMigration migration,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<TableauServerConnectionUrlFlowTransformer> logger)
        {
            _destinationConfig = migration.Plan.Destination as ITableauApiEndpointConfiguration;
            _sourceConfig = migration.Plan.Source as ITableauApiEndpointConfiguration;
            _projectFinder = destinationFinderFactory.ForDestinationContentType<IProject>();
            _logger = logger;
        }

        #endregion

        #region - JsonContentTransformerBase<IPublishableFlow> Implementation -

        /// <inheritdoc />
        protected override bool NeedsJsonTransforming(IPublishableFlow ctx)
            => ctx.Connections.Count > 0;

        /// <inheritdoc />
        public override async Task TransformAsync(IPublishableFlow ctx, JsonNode json, CancellationToken cancel)
        {
            if (_destinationConfig is null)
            {
                return;
            }

            var destConfig = _destinationConfig.SiteConnectionConfiguration;
            var destinationServerUrl = destConfig.ServerUrl.ToString().TrimEnd('/');
            var destinationSiteContentUrl = destConfig.SiteContentUrl ?? string.Empty;
            var destinationServerUrlWithSite = string.IsNullOrEmpty(destinationSiteContentUrl)
                ? destinationServerUrl
                : $"{destinationServerUrl}/#/site/{destinationSiteContentUrl}";

            var sourceServerUrl = _sourceConfig?.SiteConnectionConfiguration.ServerUrl.ToString().TrimEnd('/');
            var sourceSiteContentUrl = _sourceConfig?.SiteConnectionConfiguration.SiteContentUrl ?? string.Empty;
            string? sourceServerUrlWithSite = null;
            if (_sourceConfig is not null)
            {
                sourceServerUrlWithSite = string.IsNullOrEmpty(sourceSiteContentUrl)
                    ? sourceServerUrl
                    : $"{sourceServerUrl}/#/site/{sourceSiteContentUrl}";
            }

            var flowContainerProjectLuid = ((IContainerContent)ctx).Container.Id.ToString();
            var projectLuidCache = new Dictionary<string, string>(StringComparer.Ordinal);

            var nodes = json["nodes"] as JsonObject;
            if (nodes is not null)
            {
                foreach (var (_, node) in nodes)
                {
                    if (node is not JsonObject nodeObj) continue;

                    var baseType = nodeObj.TryGetPropertyValue("baseType", out var bt) ? bt?.GetValue<string>() : null;

                    if (baseType == "output")
                    {
                        if (!string.IsNullOrEmpty(sourceServerUrlWithSite))
                        {
                            ReplaceIfMatch(nodeObj, "serverUrl", sourceServerUrlWithSite, destinationServerUrlWithSite);
                        }
                        if (nodeObj.TryGetPropertyValue("projectLuid", out var projectLuidProp) && projectLuidProp is not null)
                        {
                            var currentLuid = projectLuidProp.GetValue<string>();
                            if (!string.IsNullOrEmpty(currentLuid))
                            {
                                if (!projectLuidCache.TryGetValue(currentLuid, out var newLuid))
                                {
                                    newLuid = flowContainerProjectLuid;
                                    if (Guid.TryParse(currentLuid, out var sourceLuid))
                                    {
                                        var destinationProject = await _projectFinder.FindBySourceIdAsync(sourceLuid, cancel).ConfigureAwait(false);
                                        newLuid = destinationProject?.Id.ToString() ?? flowContainerProjectLuid;
                                    }
                                    projectLuidCache[currentLuid] = newLuid;
                                }
                                nodeObj["projectLuid"] = newLuid;
                            }
                        }
                    }

                    if (nodeObj.TryGetPropertyValue("connectionAttributes", out var nodeAttrs) && nodeAttrs is JsonObject nodeAttrsObj)
                    {
                        ReplaceIfMatch(nodeAttrsObj, "server", sourceServerUrl ?? string.Empty, destinationServerUrl);
                        ReplaceIfMatch(nodeAttrsObj, "siteUrlName", sourceSiteContentUrl, destinationSiteContentUrl);
                    }
                }
            }

            // Input steps: server/site in top-level connections (input nodes reference these via connectionId)
            var connections = json["connections"] as JsonObject;
            if (connections is not null)
            {
                foreach (var (_, connection) in connections)
                {
                    if (connection is JsonObject connectionObj &&
                        connectionObj.TryGetPropertyValue("connectionAttributes", out var attrs) &&
                        attrs is JsonObject attrsObj)
                    {
                        ReplaceIfMatch(attrsObj, "server", sourceServerUrl ?? string.Empty, destinationServerUrl);
                        ReplaceIfMatch(attrsObj, "siteUrlName", sourceSiteContentUrl, destinationSiteContentUrl);
                    }
                }
            }
        }

        #endregion

        #region - Helpers -

        /// <summary>
        /// Replaces the value at <paramref name="key"/> with <paramref name="newValue"/> only when the current value equals <paramref name="oldValue"/> (string-replace style).
        /// </summary>
        private static void ReplaceIfMatch(JsonObject obj, string key, string oldValue, string newValue)
        {
            if (!obj.TryGetPropertyValue(key, out var prop) || prop is null) return;
            var current = prop.GetValue<string>()?.Trim().TrimEnd('/') ?? string.Empty;
            var normalizedOld = oldValue?.Trim().TrimEnd('/') ?? string.Empty;
            if (string.Compare(current, normalizedOld, StringComparison.OrdinalIgnoreCase) != 0) return;
            obj[key] = newValue;
        }

        #endregion
    }
}
