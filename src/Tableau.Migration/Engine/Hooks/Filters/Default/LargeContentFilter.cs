//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Filter that excludes content larger than a configured size threshold.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class LargeContentFilter<TContent> : ContentFilterBase<TContent>
        where TContent : IContentReference
    {
        private readonly long? _maxContentSize;

        /// <summary>
        /// Creates a new <see cref="LargeContentFilter{TContent}"/> object.
        /// </summary>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="localizer">The string localizer.</param>
        /// <param name="logger">The logger.</param>
        public LargeContentFilter(
            IConfigReader configReader,
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<TContent>> logger) : base(localizer, logger) 
        {
            var options = configReader.Get<TContent>();
            _maxContentSize = options.MaxContentSize;
        }

        /// <inheritdoc />
        public override bool ShouldMigrate(ContentMigrationItem<TContent> item)
        {
            // If no max content size is configured, allow all content
            if (_maxContentSize == null)
            {
                return true;
            }

            // Check if the content implements ISizeContent
            if (item.SourceItem is ISizeContent sizeContent)
            {
                // Filter out content larger than the configured threshold
                return sizeContent.Size <= _maxContentSize.Value;
            }
            
            return true;
        }
    }
}