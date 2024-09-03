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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class EncryptExtractTransformerTests
    {
        public abstract class EncryptExtractTransformerTest<TIExtractContent> : AutoFixtureTestBase where TIExtractContent : IContentReference, IExtractContent
        {
            protected readonly Mock<ILogger<EncryptExtractTransformer<TIExtractContent>>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();

            protected readonly Mock<IMigration> MockMigration;
            protected readonly Mock<IServerSession> MockDestinationSession;
            protected readonly Mock<IDestinationEndpoint> MockDestination;
            protected readonly Mock<ISiteSettings> MockDestinationSettings;

            protected Func<IResult<IServerSession>> DestinationSessionResult { get; set; }

            protected readonly EncryptExtractTransformer<TIExtractContent> Transformer;

            public EncryptExtractTransformerTest()
            {
                MockMigration = Create<Mock<IMigration>>();
                MockDestinationSession = Create<Mock<IServerSession>>();
                MockDestinationSettings = Create<Mock<ISiteSettings>>();

                MockDestinationSession.Setup(m => m.Settings).Returns(MockDestinationSettings.Object);

                DestinationSessionResult = () => Result<IServerSession>.Succeeded(MockDestinationSession.Object);

                MockDestination = Freeze<Mock<IDestinationEndpoint>>();
                MockDestination.Setup(x => x.GetSessionAsync(Cancel)).ReturnsAsync(() => DestinationSessionResult());

                Transformer = new(MockSharedResourcesLocalizer.Object, MockLogger.Object, MockMigration.Object);
            }
        }

        public abstract class ExecuteAsync<TIExtractContent> : EncryptExtractTransformerTest<TIExtractContent> where TIExtractContent : IContentReference, IExtractContent
        {
            [Fact]
            public async Task Returns_encrypt_extract_true_when_enforced()
            {
                MockDestinationSettings.Setup(m => m.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enforced);
                var result = await Transformer.ExecuteAsync(Create<TIExtractContent>(), Cancel);

                Assert.NotNull(result);
                Assert.True(result.EncryptExtracts);
            }

            [Fact]
            public async Task Returns_encrypt_extract_false_when_disabled()
            {
                MockDestinationSettings.Setup(m => m.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Disabled);
                var result = await Transformer.ExecuteAsync(Create<TIExtractContent>(), Cancel);

                Assert.NotNull(result);
                Assert.False(result.EncryptExtracts);
            }

            [Fact]
            public async Task Returns_encrypt_extract_same()
            {
                var data = Create<TIExtractContent>();

                var result = await Transformer.ExecuteAsync(data, Cancel);

                Assert.NotNull(result);
                Assert.Equal(data.EncryptExtracts, result.EncryptExtracts);

            }
        }

        public class DataSourceEncryptExtractTests : ExecuteAsync<IPublishableDataSource> { }

        public class WorkbookEncryptExtractTests : ExecuteAsync<IPublishableWorkbook> { }
    }
}
