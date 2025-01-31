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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// An <see cref="ISharedResourcesLocalizer"/> instance with with default behavior.
    /// </summary>
    public sealed class TestSharedResourcesLocalizer : ISharedResourcesLocalizer, IDisposable
    {
        public static readonly TestSharedResourcesLocalizer Instance = new();

        private readonly ServiceProvider _serviceProvider;
        private readonly ISharedResourcesLocalizer _localizer;

        public TestSharedResourcesLocalizer()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddLocalization()
                .AddSharedResourcesLocalization()
                .BuildServiceProvider();

            _localizer = _serviceProvider.GetRequiredService<ISharedResourcesLocalizer>();
        }

        public void Dispose() => _serviceProvider.Dispose();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);

        public LocalizedString this[string name] => _localizer[name];
        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];
    }
}
