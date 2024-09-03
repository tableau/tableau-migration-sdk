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
using System.Linq;
using Microsoft.Extensions.Options;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Methods to read the current <see cref="MigrationSdkOptions"/>.
    /// </summary>
    public class ConfigReader : IConfigReader
    {
        private readonly IOptionsMonitor<MigrationSdkOptions> _optionsMonitor;

        /// <summary>
        /// Creates a new <see cref="ConfigReader"/> object.
        /// </summary>
        /// <param name="optionsMonitor">The object to monitor configuration with.</param>
        public ConfigReader(IOptionsMonitor<MigrationSdkOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        /// <summary>
        /// Get the current <see cref="MigrationSdkOptions"/>
        /// The configuration values are auto-reloaded from supplied configuration
        /// Ex: .json file.
        /// </summary>
        /// <returns>The <see cref="MigrationSdkOptions"/></returns>
        public MigrationSdkOptions Get()
        {
            return _optionsMonitor.Get(nameof(MigrationSdkOptions));
        }

        /// <inheritdoc/>        
        public ContentTypesOptions Get<TContent>()
            where TContent : IContentReference
        {
            var contentType = ServerToCloudMigrationPipeline.ContentTypes
                .FirstOrDefault(c => c.ContentType.Name == typeof(TContent).Name);

            if (contentType != null)
            {
                var configKey = contentType.GetConfigKey();
                var contentTypeOptions = Get()
                    .ContentTypes
                    .FirstOrDefault(o => string.Equals(o.Type, configKey, StringComparison.OrdinalIgnoreCase));

                return contentTypeOptions ?? new ContentTypesOptions()
                {
                    Type = configKey
                };
            }

            throw new NotSupportedException($"Content type specific options are not supported for {typeof(TContent)} since it is not supported for migration.");
        }
    }
}
