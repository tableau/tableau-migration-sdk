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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    /// <summary>
    /// Simulation tests for error handling for various components.
    /// </summary>
    public class ErrorHandlingTests
    {
        #region - Network Layer -

        public class NetworkLayerErrorHandling : AutoFixtureTestBase
        {
            private class NetworkExceptionMessageHandler : HttpMessageHandler
            {
                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                    => throw new HttpRequestException("You shall not pass!");
            }

            [Fact]
            public async Task DoesNotCatchExceptionsAsync()
            {
                using var innerClient = new HttpClient(new NetworkExceptionMessageHandler());
                var client = new DefaultHttpClient(innerClient, Create<IHttpContentSerializer>());

                await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(Create<HttpRequestMessage>(), default));
            }
        }

        #endregion

        #region - API Layer -

        #endregion

        #region - Engine - Entry-level -

        public class EngineEntryLevelErrorHandling
        { }

        #endregion

        #region - Engine - Batch-level -

        public class EngineBatchLevelErrorHandling
        { }

        #endregion

        #region - Engine - Action-level -

        public class EngineActionLevelErrorHandling
        { }

        #endregion

        #region - Engine - Pipeline-level -

        public class EnginePipelineLevelErrorHandling
        { }

        #endregion

        #region - Engine - Migration-level -

        public class EngineMigrationErrorHandling
        { }

        #endregion
    }
}
