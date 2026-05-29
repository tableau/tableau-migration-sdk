using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Csharp.ExampleApplication.Hooks.Transformers
{
    #region class

    /// <summary>
    /// Sample JSON transformer that updates placeholder server and serverUrl in prep flow connection attributes and nodes. These may differ depending on your Tableau Server configuration
    /// and require custom tranformation, such as private network and self-hosted server configurations.
    /// Demonstrates using <see cref="JsonContentTransformerBase{TPublish}"/> for flows with performance guidance.
    /// </summary>
    public class FlowConnectionServerJsonTransformer : JsonContentTransformerBase<IPublishableFlow>
    {
        private const string PlaceholderServerUrl = "https://source-server";
        private const string DestinationServerUrl = "https://destination-server";

        protected override bool NeedsJsonTransforming(IPublishableFlow ctx)
        {
            /*
             * Returning false prevents TransformAsync from running and avoids loading the flow file.
             * Only transform flows that have connections; flows with no connections have nothing to update.
             */
            return ctx.Connections.Count > 0;
        }

        public override Task TransformAsync(IPublishableFlow ctx, JsonNode json, CancellationToken cancel)
        {
            // Changes to the JSON are saved back to the flow file before publishing.

            // Update server and serverUrl in nodes (e.g. output nodes have serverUrl; nodes may have connectionAttributes.server).
            if (json["nodes"] is JsonObject nodesObject)
            {
                foreach (var nodePair in nodesObject)
                {
                    if (nodePair.Value is not JsonObject nodeObj)
                        continue;

                    if (nodeObj["serverUrl"]?.GetValue<string>() == PlaceholderServerUrl)
                        nodeObj["serverUrl"] = DestinationServerUrl;

                    if (nodeObj["connectionAttributes"] is JsonObject nodeAttrs)
                    {
                        if (nodeAttrs["server"]?.GetValue<string>() == PlaceholderServerUrl)
                            nodeAttrs["server"] = DestinationServerUrl;
                    }
                }
            }

            // Update connectionAttributes in top-level connections (server).
            if (json["connections"] is not JsonObject connectionsObject)
                return Task.CompletedTask;

            foreach (var connectionPair in connectionsObject)
            {
                if (connectionPair.Value is not JsonObject connectionObj ||
                    connectionObj["connectionAttributes"] is not JsonObject attrs)
                    continue;

                if (attrs["server"]?.GetValue<string>() == PlaceholderServerUrl)
                    attrs["server"] = DestinationServerUrl;
            }

            return Task.CompletedTask;
        }
    }

    #endregion
}
