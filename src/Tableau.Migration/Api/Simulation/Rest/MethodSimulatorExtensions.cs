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
using System.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

namespace Tableau.Migration.Api.Simulation.Rest
{
    internal static class MethodSimulatorExtensions
    {
        public static MethodSimulator RespondWithError(this MethodSimulator method, HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            int subCode = 0,
            string summary = "Error Summary",
            string detail = "Error Detail")
        {
            return method.RespondWithError(new StaticRestErrorBuilder(statusCode, subCode, summary, detail));
        }

        public static MethodSimulator RespondWithError(this MethodSimulator method, StaticRestErrorBuilder errorBuilder)
        {
            if (method.ResponseBuilder is not IRestApiResponseBuilder restResponseBuilder)
            {
                throw new ArgumentException("Method simulator must have a REST API response builder to generate a REST API error.");
            }

            restResponseBuilder.ErrorOverride = errorBuilder;
            return method;
        }
    }
}
