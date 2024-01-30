// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Interop;
using Tableau.Migration.Interop.Logging;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class SdkUserAgentTests
    {
        private readonly IServiceCollection _servicesCollection;
        private ServiceProvider _services;

        public SdkUserAgentTests()
        {
            _servicesCollection = new ServiceCollection()
                .AddTableauMigrationSdk();
            _services = _servicesCollection.BuildServiceProvider();
        }

        [Fact]
        public void DefaultValues()
        {
            var sdkMetaData = _services.GetRequiredService<IMigrationSdk>();
            var id = sdkMetaData.UserAgent;
            Assert.NotNull(id);
            Assert.NotEmpty(id);
            Assert.StartsWith(Constants.USER_AGENT_PREFIX, id);

            Assert.Contains(sdkMetaData.Version.ToString(), id);
        }

        [Fact]
        public void PythonValues()
        {
            var mockLoggerFactory = new Mock<Func<IServiceProvider, NonGenericLoggerProvider>>();

            _servicesCollection.AddPythonSupport(mockLoggerFactory.Object);
            _services = _servicesCollection.BuildServiceProvider();

            var sdkMetaData = _services.GetRequiredService<IMigrationSdk>();
            var id = sdkMetaData.UserAgent;
            Assert.NotNull(id);
            Assert.NotEmpty(id);
            Assert.StartsWith(Constants.USER_AGENT_PREFIX, id);
            Assert.Contains(Constants.USER_AGENT_PYTHON_SUFFIX, id);

            Assert.Contains(sdkMetaData.Version.ToString(), id);
        }
    }
}
