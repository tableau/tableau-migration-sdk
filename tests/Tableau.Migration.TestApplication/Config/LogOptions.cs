﻿//
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

using Serilog.Events;

namespace Tableau.Migration.TestApplication.Config
{
    public class LogOptions
    {
        public string FolderPath { get; set; } = string.Empty;

        public string ManifestFolderPath { get; set; } = string.Empty;

        public MinimumLogLevelOverride[] MinLogLevelOverrides { get; set; } = [];

        public string[] ConsoleLoggingSources { get; set; } = [];

        public int FileSizeLimitMB { get; set; } = 250;

        public class MinimumLogLevelOverride
        {
            public string Source { get; set; } = string.Empty;

            public LogEventLevel Level { get; set; }
        }
    }
}
