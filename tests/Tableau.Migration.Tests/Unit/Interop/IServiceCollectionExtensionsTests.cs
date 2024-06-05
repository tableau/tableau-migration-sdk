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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Interop;
using Tableau.Migration.Interop.Logging;

namespace Tableau.Migration.Tests.Unit.Interop
{
    public class IServiceCollectionExtensionsTests
    {
        public class AddPythonSupport : IServiceCollectionExtensionsTestBase
        {
            class PythonLogger : NonGenericLoggerBase
            {
                public override bool IsEnabled(LogLevel logLevel) => true;

                public override void Log(
                    LogLevel logLevel,
                    EventId eventId,
                    string state,
                    Exception? exception,
                    string message)
                { }
            }

            PythonLogger GetLogger(string name)
                => new PythonLogger();

            protected override void ConfigureServices(IServiceCollection services)
            {
                services.AddPythonSupport(GetLogger);
            }
        }
    }
}
