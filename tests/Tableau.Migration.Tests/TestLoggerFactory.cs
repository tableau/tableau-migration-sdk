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
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tableau.Migration.Tests
{
    public class TestLoggerFactory : Mock<ILoggerFactory>, ILoggerFactory
    {
        private readonly ConcurrentDictionary<string, TestLogger> _loggersByCategory = new();

        public TestLoggerFactory()
        {
            Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns((string category) => _loggersByCategory.GetOrAdd(category, _ => new TestLogger()));
        }

        #region - ILoggerFactory -

        ILogger ILoggerFactory.CreateLogger(string categoryName) => Object.CreateLogger(categoryName);

        void ILoggerFactory.AddProvider(ILoggerProvider provider) => Object.AddProvider(provider);

        void IDisposable.Dispose()
        {
            Object.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
