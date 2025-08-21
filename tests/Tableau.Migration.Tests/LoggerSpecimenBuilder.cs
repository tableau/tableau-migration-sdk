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
using System.Linq;
using AutoFixture.Kernel;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tableau.Migration.Tests
{
    internal sealed class LoggerSpecimenBuilder : ISpecimenBuilder
    {
        private readonly TestLoggerFactory _loggerFactory;

        public LoggerSpecimenBuilder()
        {
            _loggerFactory = new();
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t)
            {
                return new NoSpecimen();
            }

            if(t == typeof(TestLoggerFactory) || t == typeof(Mock<ILoggerFactory>))
            {
                return _loggerFactory;
            }
            else if(t == typeof(ILoggerFactory))
            {
                return _loggerFactory.Object;
            }
            else if(t == typeof(Mock<ILogger>))
            {
                return _loggerFactory.DefaultLogger;
            }
            else if(t == typeof(ILogger))
            {
                return _loggerFactory.DefaultLogger.Object;
            }
            else if(t.IsGenericType)
            {
                var genericType = t.GetGenericTypeDefinition();

                if (genericType == typeof(ILogger<>))
                {
                    return typeof(TestLoggerFactory).GetMethod(nameof(TestLoggerFactory.CreateTestLogger))
                        !.MakeGenericMethod(t.GenericTypeArguments.Single()).Invoke(_loggerFactory, null)!;
                }
                else if(genericType == typeof(Mock<>))
                {
                    var mockType = t.GenericTypeArguments.First();
                    if(mockType.IsGenericType && mockType.GetGenericTypeDefinition() == typeof(ILogger<>))
                    {
                        return typeof(TestLoggerFactory).GetMethod(nameof(TestLoggerFactory.CreateTestLogger))
                            !.MakeGenericMethod(mockType.GenericTypeArguments.Single()).Invoke(_loggerFactory, null)!;
                    }
                }
            }

            return new NoSpecimen();
        }
    }
}
