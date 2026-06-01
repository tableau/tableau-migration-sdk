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
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class TableauServerConnectionUrlFlowTransformerTests
    {
        public class TestTableauServerConnectionUrlFlowTransformer : TableauServerConnectionUrlFlowTransformer
        {
            public TestTableauServerConnectionUrlFlowTransformer(
                IMigration migration,
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ILogger<TableauServerConnectionUrlFlowTransformer> logger)
                : base(migration, destinationFinderFactory, logger)
            { }

            public bool PublicNeedsJsonTransforming(IPublishableFlow ctx)
                => base.NeedsJsonTransforming(ctx);
        }

        public class TableauServerConnectionUrlFlowTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinder<IProject>> MockProjectFinder;
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationFinderFactory;
            protected readonly Mock<ILogger<TableauServerConnectionUrlFlowTransformer>> MockLogger;
            protected readonly Mock<IPublishableFlow> MockFlow;

            protected readonly Mock<ITableauApiEndpointConfiguration> MockDestinationConfig;
            protected TableauSiteConnectionConfiguration DestinationSiteConfig;

            protected readonly Mock<ITableauApiEndpointConfiguration>? MockSourceConfig;
            protected TableauSiteConnectionConfiguration? SourceSiteConfig;

            protected readonly TestTableauServerConnectionUrlFlowTransformer Transformer;

            public TableauServerConnectionUrlFlowTransformerTest()
            {
                MockProjectFinder = Freeze<Mock<IDestinationContentReferenceFinder<IProject>>>();
                MockProjectFinder.Setup(x => x.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Guid sourceId, CancellationToken _) => null);

                MockDestinationFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
                MockDestinationFinderFactory.Setup(x => x.ForDestinationContentType<IProject>())
                    .Returns(MockProjectFinder.Object);

                MockLogger = Freeze<Mock<ILogger<TableauServerConnectionUrlFlowTransformer>>>();

                DestinationSiteConfig = new TableauSiteConnectionConfiguration(
                    new Uri("https://destServer"),
                    "destSite",
                    "token",
                    "token");
                SourceSiteConfig = new TableauSiteConnectionConfiguration(
                    new Uri("https://sourceServer"),
                    "sourceSite",
                    "token",
                    "token");
                // Use Create for BOTH so we get two distinct mocks. Freeze<Mock<ITableauApiEndpointConfiguration>>()
                // would make the second Create return the same instance, so the second SetupGet would overwrite the first.
                MockDestinationConfig = Create<Mock<ITableauApiEndpointConfiguration>>();
                MockDestinationConfig.SetupGet(x => x.SiteConnectionConfiguration)
                    .Returns(() => DestinationSiteConfig);
                MockSourceConfig = Create<Mock<ITableauApiEndpointConfiguration>>();
                MockSourceConfig.SetupGet(x => x.SiteConnectionConfiguration)
                    .Returns(() => SourceSiteConfig!.Value);

                var mockMigration = Freeze<Mock<IMigration>>();
                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Destination).Returns(MockDestinationConfig.Object);
                mockPlan.SetupGet(x => x.Source).Returns(MockSourceConfig.Object);
                mockMigration.SetupGet(x => x.Plan).Returns(mockPlan.Object);

                MockFlow = Create<Mock<IPublishableFlow>>();
                var containerRef = Create<Mock<IContentReference>>();
                var containerId = Create<Guid>();
                containerRef.SetupGet(x => x.Id).Returns(containerId);
                MockFlow.As<IContainerContent>().SetupGet(x => x.Container).Returns(containerRef.Object);

                Transformer = Create<TestTableauServerConnectionUrlFlowTransformer>();
            }

            protected void SetFlowConnections(int count)
            {
                var connections = CreateMany<Mock<IConnection>>(count).Select(m => m.Object).ToImmutableArray();
                MockFlow.SetupGet(x => x.Connections).Returns(connections);
            }

            protected IContentReference SetupProjectFinder(Guid sourceProjectId, Guid? destinationProjectId = null)
            {
                var destRef = Create<Mock<IContentReference>>();
                destRef.SetupGet(x => x.Id).Returns(destinationProjectId ?? Create<Guid>());
                MockProjectFinder.Setup(x => x.FindBySourceIdAsync(sourceProjectId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(destRef.Object);
                return destRef.Object;
            }
        }

        public class NeedsJsonTransforming : TableauServerConnectionUrlFlowTransformerTest
        {
            [Fact]
            public void HasConnections()
            {
                SetFlowConnections(1);
                Assert.True(Transformer.PublicNeedsJsonTransforming(MockFlow.Object));
            }

            [Fact]
            public void NoConnections()
            {
                SetFlowConnections(0);
                Assert.False(Transformer.PublicNeedsJsonTransforming(MockFlow.Object));
            }
        }

        public class TransformAsync : TableauServerConnectionUrlFlowTransformerTest
        {
            [Fact]
            public async Task NoOpWhenDestinationConfigIsNullAsync()
            {
                var mockPlan = Create<Mock<IMigrationPlan>>();
                var nonTableauDestination = Create<Mock<IMigrationPlanEndpointConfiguration>>();
                mockPlan.SetupGet(x => x.Destination).Returns(nonTableauDestination.Object);
                mockPlan.SetupGet(x => x.Source).Returns(MockSourceConfig!.Object);
                var mockMigration = Create<Mock<IMigration>>();
                mockMigration.SetupGet(x => x.Plan).Returns(mockPlan.Object);

                var transformer = new TestTableauServerConnectionUrlFlowTransformer(
                    mockMigration.Object,
                    MockDestinationFinderFactory.Object,
                    MockLogger.Object);

                SetFlowConnections(1);
                var json = JsonNode.Parse("{\"nodes\":{\"n1\":{\"baseType\":\"output\",\"serverUrl\":\"https://sourceServer/#/site/sourceSite\"}}}")!.AsObject();

                await transformer.TransformAsync(MockFlow.Object, json, Cancel);

                Assert.Equal("https://sourceServer/#/site/sourceSite", json["nodes"]!["n1"]!["serverUrl"]?.GetValue<string>());
            }

            [Fact]
            public async Task UpdatesOutputNodeServerUrlAndProjectLuidAsync()
            {
                SetFlowConnections(1);
                var sourceProjectId = Create<Guid>();
                var destProjectId = Create<Guid>();
                SetupProjectFinder(sourceProjectId, destProjectId);

                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""out1"": {
                            ""baseType"": ""output"",
                            ""serverUrl"": ""https://sourceServer/#/site/sourceSite"",
                            ""projectLuid"": """ + sourceProjectId + @"""
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var outNode = json["nodes"]!["out1"]!.AsObject();
                Assert.Equal("https://destserver/#/site/destSite", outNode["serverUrl"]?.GetValue<string>());
                Assert.Equal(destProjectId.ToString(), outNode["projectLuid"]?.GetValue<string>());
            }

            [Fact]
            public async Task ReplacesOnlyWhenSourceMatchesAsync()
            {
                SetFlowConnections(1);
                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""out1"": {
                            ""baseType"": ""output"",
                            ""serverUrl"": ""https://otherServer/#/site/otherSite""
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var outNode = json["nodes"]!["out1"]!.AsObject();
                Assert.Equal("https://otherServer/#/site/otherSite", outNode["serverUrl"]?.GetValue<string>());
            }

            [Fact]
            public async Task UpdatesConnectionAttributesInNodesAsync()
            {
                SetFlowConnections(1);
                var sourceServerUrl = SourceSiteConfig!.Value.ServerUrl.ToString().TrimEnd('/');
                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""n1"": {
                            ""connectionAttributes"": {
                                ""server"": """ + sourceServerUrl + @""",
                                ""siteUrlName"": ""sourceSite""
                            }
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var attrs = json["nodes"]!["n1"]!["connectionAttributes"]!.AsObject();
                Assert.Equal(DestinationSiteConfig.ServerUrl.ToString().TrimEnd('/'), attrs["server"]?.GetValue<string>());
                Assert.Equal("destSite", attrs["siteUrlName"]?.GetValue<string>());
            }

            [Fact]
            public async Task UpdatesTopLevelConnectionsAsync()
            {
                SetFlowConnections(1);
                var sourceServerUrl = SourceSiteConfig!.Value.ServerUrl.ToString().TrimEnd('/');
                var json = JsonNode.Parse(@"{
                    ""nodes"": {},
                    ""connections"": {
                        ""conn1"": {
                            ""connectionAttributes"": {
                                ""server"": """ + sourceServerUrl + @""",
                                ""siteUrlName"": ""sourceSite""
                            }
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var attrs = json["connections"]!["conn1"]!["connectionAttributes"]!.AsObject();
                Assert.Equal(DestinationSiteConfig.ServerUrl.ToString().TrimEnd('/'), attrs["server"]?.GetValue<string>());
                Assert.Equal("destSite", attrs["siteUrlName"]?.GetValue<string>());
            }

            [Fact]
            public async Task UsesFlowContainerProjectLuidWhenProjectFinderReturnsNullAsync()
            {
                SetFlowConnections(1);
                var containerRef = Create<Mock<IContentReference>>();
                var containerId = Create<Guid>();
                containerRef.SetupGet(x => x.Id).Returns(containerId);
                MockFlow.As<IContainerContent>().SetupGet(x => x.Container).Returns(containerRef.Object);

                var sourceProjectId = Create<Guid>();
                MockProjectFinder.Setup(x => x.FindBySourceIdAsync(sourceProjectId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((IContentReference?)null);

                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""out1"": {
                            ""baseType"": ""output"",
                            ""projectLuid"": """ + sourceProjectId + @"""
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var outNode = json["nodes"]!["out1"]!.AsObject();
                Assert.Equal(containerId.ToString(), outNode["projectLuid"]?.GetValue<string>());
            }

            [Fact]
            public async Task HandlesMissingNodesAndConnectionsAsync()
            {
                SetFlowConnections(1);
                var json = new JsonObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                Assert.Null(json["nodes"]);
                Assert.Null(json["connections"]);
            }

            [Fact]
            public async Task NormalizesTrailingSlashInReplaceIfMatchAsync()
            {
                SetFlowConnections(1);
                var destConfigWithSlash = new TableauSiteConnectionConfiguration(
                    new Uri("https://destServer/"),
                    "destSite",
                    "token",
                    "token");
                var sourceConfigWithSlash = new TableauSiteConnectionConfiguration(
                    new Uri("https://sourceServer/"),
                    "sourceSite",
                    "token",
                    "token");

                var mockDestConfig = new Mock<ITableauApiEndpointConfiguration>();
                mockDestConfig.SetupGet(x => x.SiteConnectionConfiguration).Returns(destConfigWithSlash);
                var mockSourceConfig = new Mock<ITableauApiEndpointConfiguration>();
                mockSourceConfig.SetupGet(x => x.SiteConnectionConfiguration).Returns(sourceConfigWithSlash);
                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.Destination).Returns(mockDestConfig.Object);
                mockPlan.SetupGet(x => x.Source).Returns(mockSourceConfig.Object);
                var mockMigration = Create<Mock<IMigration>>();
                mockMigration.SetupGet(x => x.Plan).Returns(mockPlan.Object);
                var transformer = new TestTableauServerConnectionUrlFlowTransformer(
                    mockMigration.Object,
                    MockDestinationFinderFactory.Object,
                    MockLogger.Object);

                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""n1"": {
                            ""connectionAttributes"": {
                                ""server"": ""https://sourceServer/"",
                                ""siteUrlName"": ""sourceSite""
                            }
                        }
                    }
                }")!.AsObject();
                Console.WriteLine($"[Test] JSON before transform: server={json["nodes"]?["n1"]?["connectionAttributes"]?["server"]?.GetValue<string>()}");

                await transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var attrs = json["nodes"]!["n1"]!["connectionAttributes"]!.AsObject();
                var serverAfter = attrs["server"]?.GetValue<string>();
                var siteUrlNameAfter = attrs["siteUrlName"]?.GetValue<string>();
                Console.WriteLine($"[Test] JSON after transform: server={serverAfter}, siteUrlName={siteUrlNameAfter}");
                Assert.Equal("https://destserver", serverAfter);
                Assert.Equal("destSite", siteUrlNameAfter);
            }

            [Fact]
            public async Task SkipsNonOutputNodesForServerUrlAndProjectLuidAsync()
            {
                SetFlowConnections(1);
                var json = JsonNode.Parse(@"{
                    ""nodes"": {
                        ""input1"": {
                            ""baseType"": ""input"",
                            ""serverUrl"": ""https://sourceServer/#/site/sourceSite""
                        }
                    }
                }")!.AsObject();

                await Transformer.TransformAsync(MockFlow.Object, json, Cancel);

                var inputNode = json["nodes"]!["input1"]!.AsObject();
                Assert.Equal("https://sourceServer/#/site/sourceSite", inputNode["serverUrl"]?.GetValue<string>());
            }
        }
    }
}
