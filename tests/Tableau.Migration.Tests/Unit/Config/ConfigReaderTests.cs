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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class ConfigReaderTests : AutoFixtureTestBase
    {

        private readonly Mock<IOptionsMonitor<MigrationSdkOptions>> MockOptionsMonitor;
        private readonly ConfigReader Reader;

        public ConfigReaderTests()
        {
            MockOptionsMonitor = new Mock<IOptionsMonitor<MigrationSdkOptions>>();
            MockOptionsMonitor.Setup(x => x.Get(nameof(MigrationSdkOptions))).Returns(new MigrationSdkOptions());
            Reader = new ConfigReader(MockOptionsMonitor.Object);
        }
        protected void SetupOptionsMonitor()
        {
            MockOptionsMonitor.Setup(x => x.Get(nameof(MigrationSdkOptions)))
                .Returns(
                new MigrationSdkOptions()
                {
                    ContentTypes = GetContentTypesOptionsTestData()
                });
        }

        protected static List<ContentTypesOptions> GetContentTypesOptionsTestData()
            => [
                new()
                {
                    Type = "User",
                    BatchSize = 223
                },
                new()
                {
                    Type = "Group",
                    BatchSize = 224
                },
                new()
                {
                    Type = "Project",
                    BatchSize = 225
                },
                new()
                {
                    Type = "Workbook",
                    BatchSize = 226,
                    IncludeExtractEnabled = false
                },
                new()
                {
                    Type = "DataSource",
                    BatchSize = 227,
                    IncludeExtractEnabled = false
                }

            ];

        public void AssertCustomBatchSizeResult(ContentTypesOptions expected, ContentTypesOptions actual)
        {
            Assert.Equal(expected.BatchSize, actual.BatchSize);
        }
        
        public void AssertCustomIncludeExtractResult(ContentTypesOptions expected, ContentTypesOptions actual)
        {
            Assert.Equal(expected.IncludeExtractEnabled, actual.IncludeExtractEnabled);
        }

        public void AssertDefaultResult(ContentTypesOptions actual)
        {
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_SIZE, actual.BatchSize);
            Assert.Equal(ContentTypesOptions.Defaults.BATCH_PUBLISHING_ENABLED, actual.BatchPublishingEnabled);
            Assert.Equal(ContentTypesOptions.Defaults.INCLUDE_EXTRACT_ENABLED, actual.IncludeExtractEnabled);
        }

        public class GetContentTypeSpecific : ConfigReaderTests
        {
            [Fact]
            public void GetsCustomValues()
            {
                SetupOptionsMonitor();

                var testData = GetContentTypesOptionsTestData();

                AssertCustomBatchSizeResult(testData.First(i => i.Type == "User"), Reader.Get<IUser>());
                AssertCustomBatchSizeResult(testData.First(i => i.Type == "Group"), Reader.Get<IGroup>());
                AssertCustomBatchSizeResult(testData.First(i => i.Type == "Project"), Reader.Get<IProject>());
                AssertCustomBatchSizeResult(testData.First(i => i.Type == "Workbook"), Reader.Get<IWorkbook>());
                AssertCustomBatchSizeResult(testData.First(i => i.Type == "DataSource"), Reader.Get<IDataSource>());
                
                AssertCustomIncludeExtractResult(testData.First(i => i.Type == "Workbook"), Reader.Get<IWorkbook>());
                AssertCustomIncludeExtractResult(testData.First(i => i.Type == "DataSource"), Reader.Get<IDataSource>());
            }

            [Fact]
            public void GetsDefaultValues()
            {
                AssertDefaultResult(Reader.Get<IUser>());
                AssertDefaultResult(Reader.Get<IGroup>());
                AssertDefaultResult(Reader.Get<IProject>());
                AssertDefaultResult(Reader.Get<IWorkbook>());
                AssertDefaultResult(Reader.Get<IDataSource>());
            }
        }
    }
}
