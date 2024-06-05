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

namespace Tableau.Migration.TestApplication.Config
{
    public sealed class TestApplicationOptions
    {
        public EndpointOptions Source { get; set; } = new();

        public EndpointOptions Destination { get; set; } = new();

        public DomainOptions Domain { get; set; } = new();

        public LogOptions Log { get; set; } = new();

        public SpecialUsersOptions SpecialUsers { get; set; } = new();

        public string PreviousManifestPath { get; set; } = "";

        public string SkippedProject { get; set; } = string.Empty;

        public string SkippedMissingParentDestination { get; set; } = "Missing Parent";
    }
}
