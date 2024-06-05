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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class TableauServerConnectionUrlTransformerTests
    {
        public class TestTableauServerConnectionUrlTransformer : TableauServerConnectionUrlTransformer
        {
            public TestTableauServerConnectionUrlTransformer(
                IMigration migration,
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ILogger<TableauServerConnectionUrlTransformer> logger, 
                ISharedResourcesLocalizer localizer)
                : base(migration, destinationFinderFactory, logger, localizer)
            { }

            public bool PublicNeedsXmlTransforming(IPublishableWorkbook ctx)
                => base.NeedsXmlTransforming(ctx);
        }

        public class TableauServerConnectionUrlTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinder<IDataSource>> MockDataSourceFinder;
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationFinderFactory;
            protected readonly Mock<ILogger<TableauServerConnectionUrlTransformer>> MockLog;
            protected readonly MockSharedResourcesLocalizer MockLocalizer;

            protected readonly Mock<IPublishableWorkbook> MockWorkbook;
            protected readonly Dictionary<string, IContentReference> DataSourceReferencesBySourceContentUrl;

            protected readonly Mock<ITableauApiEndpointConfiguration> MockDestinationApiEndpointConfig;
            protected TableauSiteConnectionConfiguration DestinationConfig { get; set; }

            protected readonly TestTableauServerConnectionUrlTransformer Transformer;

            protected virtual IMigrationPlanEndpointConfiguration GetDestinationEndpointConfig()
                => MockDestinationApiEndpointConfig.Object;

            public TableauServerConnectionUrlTransformerTest()
            {
                MockDataSourceFinder = Freeze<Mock<IDestinationContentReferenceFinder<IDataSource>>>();
                MockLog = Freeze<Mock<ILogger<TableauServerConnectionUrlTransformer>>>();
                MockLocalizer = Freeze<MockSharedResourcesLocalizer>();

                MockWorkbook = Create<Mock<IPublishableWorkbook>>();

                DataSourceReferencesBySourceContentUrl = new();
                MockDataSourceFinder.Setup(x => x.FindBySourceContentUrlAsync(It.IsAny<string>(), Cancel))
                    .ReturnsAsync((string contentUrl, CancellationToken cancel) =>
                    {
                        if (DataSourceReferencesBySourceContentUrl.TryGetValue(contentUrl, out var val))
                        {
                            return val;
                        }

                        return null;
                    });

                MockDestinationFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
                MockDestinationFinderFactory.Setup(x => x.ForDestinationContentType<IDataSource>())
                    .Returns(MockDataSourceFinder.Object);

                DestinationConfig = Create<TableauSiteConnectionConfiguration>();

                MockDestinationApiEndpointConfig = Freeze<Mock<ITableauApiEndpointConfiguration>>();
                MockDestinationApiEndpointConfig.SetupGet(x => x.SiteConnectionConfiguration)
                    .Returns(() => DestinationConfig);

                var mockMigration = Freeze<Mock<IMigration>>();
                mockMigration.Setup(x => x.Plan.Destination).Returns(GetDestinationEndpointConfig);

                Transformer = Create<TestTableauServerConnectionUrlTransformer>();
            }

            protected IContentReference AddDataSourceReference(string sourceContentUrl)
            {
                var resultRef = Create<IContentReference>();
                DataSourceReferencesBySourceContentUrl[sourceContentUrl] = resultRef;
                return resultRef;
            }
        }

        public class NeedsXmlTransforming : TableauServerConnectionUrlTransformerTest
        {
            [Fact]
            public void HasTableauServerConnection()
            {
                var mockConnections = CreateMany<Mock<IConnection>>().ToImmutableArray();
                mockConnections[1].SetupGet(x => x.Type).Returns(TableauServerConnectionUrlTransformer.TABLEAU_SERVER_CONNECTION_CLASS);

                var mockWorkbook = Create<Mock<IPublishableWorkbook>>();
                mockWorkbook.SetupGet(x => x.Connections).Returns(mockConnections.Select(mc => mc.Object).ToImmutableArray());

                Assert.True(Transformer.PublicNeedsXmlTransforming(mockWorkbook.Object));
            }

            [Fact]
            public void NoTableauServerConnection()
            {
                var mockConnections = CreateMany<Mock<IConnection>>().ToImmutableArray();

                var mockWorkbook = Create<Mock<IPublishableWorkbook>>();
                mockWorkbook.SetupGet(x => x.Connections).Returns(mockConnections.Select(mc => mc.Object).ToImmutableArray());

                Assert.False(Transformer.PublicNeedsXmlTransforming(mockWorkbook.Object));
            }
        }

        public class ExecuteAsync : TableauServerConnectionUrlTransformerTest
        {
            [Fact]
            public async Task UpdatesConnectionAndRepoLocationAsync()
            {
                var pdsRef1 = AddDataSourceReference("MyDataSource");

                DestinationConfig = new(new Uri("https://destServer"), "dest", "", "");

                //Data source with single PDS reference connection.
                var dsRepo = XElement.Parse("<repository-location id='MyDataSource' path='/t/test/datasources' revision='1.0' site='test' />");
                var dsConn = XElement.Parse("<connection class='sqlproxy' channel='http' dbname='MyDataSource' port='80' server='localhost' />");

                var ds = XElement.Parse("<datasource />");
                ds.Add(dsRepo, dsConn);

                var dses = XElement.Parse("<datasources />");
                dses.Add(ds);

                var wb = XElement.Parse("<workbook />");
                wb.Add(dses);

                var xml = new XDocument(wb);

                //Transform
                await Transformer.TransformAsync(MockWorkbook.Object, xml, Cancel);

                //Assert
                Assert.Equal(pdsRef1.ContentUrl, dsRepo.Attribute("id")!.Value);
                Assert.Equal($"/t/{DestinationConfig.SiteContentUrl}/datasources", dsRepo.Attribute("path")!.Value);
                Assert.Equal(DestinationConfig.SiteContentUrl, dsRepo.Attribute("site")!.Value);

                Assert.Equal("https", dsConn.Attribute("channel")!.Value);
                Assert.Equal(pdsRef1.ContentUrl, dsConn.Attribute("dbname")!.Value);
                Assert.Equal("443", dsConn.Attribute("port")!.Value);
                Assert.Equal(DestinationConfig.ServerUrl.Host, dsConn.Attribute("server")!.Value);
            }

            [Fact]
            public async Task UpdatesAllDataSourcesAsync()
            {
                var pdsRef1 = AddDataSourceReference("MyDataSource");
                var pdsRef2 = AddDataSourceReference("AnotherDataSource");

                var ds1Repo = XElement.Parse("<repository-location id='MyDataSource' path='/t/test/datasources' revision='1.0' site='test' />");
                var ds1Conn = XElement.Parse("<connection class='sqlproxy' channel='http' dbname='MyDataSource' port='80' server='localhost' />");
                var ds1 = XElement.Parse("<datasource />");
                ds1.Add(ds1Repo, ds1Conn);

                var ds2Repo = XElement.Parse("<repository-location id='AnotherDataSource' path='/t/test/datasources' revision='1.0' site='test' />");
                var ds2Conn = XElement.Parse("<connection class='sqlproxy' channel='http' dbname='AnotherDataSource' port='80' server='localhost' />");
                var ds2 = XElement.Parse("<datasource />");
                ds2.Add(ds2Repo, ds2Conn);

                var dses = XElement.Parse("<datasources />");
                dses.Add(ds1, ds2);

                var wb = XElement.Parse("<workbook />");
                wb.Add(dses);

                var xml = new XDocument(wb);

                //Transform
                await Transformer.TransformAsync(MockWorkbook.Object, xml, Cancel);

                //Assert
                Assert.Equal(pdsRef1.ContentUrl, ds1Repo.Attribute("id")!.Value);
                Assert.Equal(pdsRef1.ContentUrl, ds1Conn.Attribute("dbname")!.Value);

                Assert.Equal(pdsRef2.ContentUrl, ds2Repo.Attribute("id")!.Value);
                Assert.Equal(pdsRef2.ContentUrl, ds2Conn.Attribute("dbname")!.Value);
            }

            [Fact]
            public async Task OnlyUpdatesTableauServerConnectionsAsync()
            {
                var pdsRef1 = AddDataSourceReference("MyDataSource");

                DestinationConfig = new(new Uri("https://destServer"), "dest", "", "");

                //Data source with single PDS reference connection.
                var dsConn = XElement.Parse("<connection class='sqlserver' channel='http' dbname='MyDataSource' port='80' server='localhost' />");

                var ds = XElement.Parse("<datasource />");
                ds.Add(dsConn);

                var dses = XElement.Parse("<datasources />");
                dses.Add(ds);

                var wb = XElement.Parse("<workbook />");
                wb.Add(dses);

                var xml = new XDocument(wb);

                //Transform
                await Transformer.TransformAsync(MockWorkbook.Object, xml, Cancel);

                //Assert
                Assert.Equal("http", dsConn.Attribute("channel")!.Value);
                Assert.Equal("MyDataSource", dsConn.Attribute("dbname")!.Value);
                Assert.Equal("80", dsConn.Attribute("port")!.Value);
                Assert.Equal("localhost", dsConn.Attribute("server")!.Value);
            }

            [Fact]
            public async Task WarnsMissingReferenceSingleTimePerWorkbookAndContentUrl()
            {
                var ds1Repo = XElement.Parse("<repository-location id='MyDataSource' />");
                var ds1Conn = XElement.Parse("<connection class='sqlproxy' dbname='MyDataSource' />");
                var ds1 = XElement.Parse("<datasource />");
                ds1.Add(ds1Repo, ds1Conn);

                var ds2Repo = XElement.Parse("<repository-location id='AnotherDataSource' />");
                var ds2Conn = XElement.Parse("<connection class='sqlproxy' dbname='AnotherDataSource' />");
                var ds2 = XElement.Parse("<datasource />");
                ds2.Add(ds2Repo, ds2Conn);

                var ds3Repo = XElement.Parse("<repository-location id='AnotherDataSource' />");
                var ds3Conn = XElement.Parse("<connection class='sqlproxy' dbname='AnotherDataSource' />");
                var ds3 = XElement.Parse("<datasource />");
                ds3.Add(ds3Repo, ds3Conn);

                var dses = XElement.Parse("<datasources />");
                dses.Add(ds1, ds2);

                var wb = XElement.Parse("<workbook />");
                wb.Add(dses);

                var xml = new XDocument(wb);

                //Transform
                await Transformer.TransformAsync(MockWorkbook.Object, xml, Cancel);

                var mockWorkbook2 = Create<Mock<IPublishableWorkbook>>();
                await Transformer.TransformAsync(mockWorkbook2.Object, xml, Cancel);

                //Assert
                MockLog.VerifyWarnings(Times.Exactly(4));
            }
        }
    }
}
