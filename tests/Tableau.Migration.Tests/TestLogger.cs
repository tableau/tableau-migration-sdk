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
using Microsoft.Extensions.Logging;
using Moq;

namespace Tableau.Migration.Tests
{
    public class TestLogger : TestLogger<string>
    { 
    }

    public class TestLogger<TCategoryName> : Mock<ILogger<TCategoryName>>, ILogger<TCategoryName>
    {
        private readonly ImmutableArray<TestLoggerMessage>.Builder _messages = ImmutableArray.CreateBuilder<TestLoggerMessage>();

        public IImmutableList<TestLoggerMessage> Messages => _messages.ToImmutable();

        public TestLogger()
        {
            Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsSubtype<IReadOnlyList<KeyValuePair<string, object>>>>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsSubtype<IReadOnlyList<KeyValuePair<string, object>>>, Exception?, string>>()))
                .Callback<LogLevel, EventId, object, Exception?, Delegate>((logLevel, eventId, state, exception, formatter) =>
                {
                    var formatted = formatter.DynamicInvoke(state, exception)?.ToString();

                    var logMessage = new TestLoggerMessage(formatted, (IReadOnlyList<KeyValuePair<string, object>>)state, logLevel, eventId, exception);

                    _messages.Add(logMessage);
                });
        }

        #region - ILogger -

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Object.Log(logLevel, eventId, state, exception, formatter);

        bool ILogger.IsEnabled(LogLevel logLevel) => Object.IsEnabled(logLevel);

        IDisposable? ILogger.BeginScope<TState>(TState state) => Object.BeginScope(state);

        #endregion
    }
}
