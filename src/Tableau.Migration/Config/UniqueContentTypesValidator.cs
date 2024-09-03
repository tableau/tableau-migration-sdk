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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Options;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Config
{
    internal sealed class UniqueContentTypesValidator : IValidateOptions<MigrationSdkOptions>
    {
        private readonly ISharedResourcesLocalizer _localizer;

        public UniqueContentTypesValidator(ISharedResourcesLocalizer localizer)
        {
            _localizer = localizer;
        }

        public ValidateOptionsResult Validate(string? name, MigrationSdkOptions options)
        {
            var duplicateContentErrors = options.ContentTypes
                .GroupBy(v => v.Type, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.First().IsContentTypeValid() && g.Count() > 1)
                .Select(g => string.Format(_localizer[SharedResourceKeys.DuplicateContentTypeConfigurationMessage], g.Key))
                .ToImmutableArray();

            if (duplicateContentErrors.Any())
            {
                return ValidateOptionsResult.Fail(duplicateContentErrors);
            }

            return ValidateOptionsResult.Success;
        }
    }
}
