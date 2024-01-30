﻿// Copyright (c) 2023, Salesforce, Inc.
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
using System.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class BuildResponseException : Exception
    {
        public BuildResponseException(HttpStatusCode statusCode, int subCode, string summary, string detail)
        {
            StatusCode = statusCode;
            SubCode = subCode;
            Summary = summary;
            Detail = detail;
        }

        public HttpStatusCode StatusCode { get; set; }
        public int SubCode { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }

    }
}
