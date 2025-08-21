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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Resources
{
    public class SharedResourceTests
    {
        public class LocalizedClass
        {
            private readonly ISharedResourcesLocalizer _localizer;

            public LocalizedClass(ISharedResourcesLocalizer localizer)
            {
                _localizer = localizer;
            }

            public string GetLocalizedString() => _localizer[SharedResourceKeys.ProductName];

            public string GetLocalizedString(string key) => _localizer[key];
        }

        private static ServiceCollection CreateLocalizationTestServices()
        {
            var services = new ServiceCollection();
            services.AddTableauMigrationSdk();
            services.AddTransient<LocalizedClass>();

            return services;
        }

        public class ISharedResourcesLocalizerTests
        {
            [Fact]
            public void Default()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var loc = container.GetRequiredService<LocalizedClass>();

                Assert.Equal("Tableau Migration SDK", loc.GetLocalizedString());
            }

            [Fact]
            public void CustomResourcePath()
            {
                var services = CreateLocalizationTestServices();
                services.AddLocalization(opts => opts.ResourcesPath = "SpecialPath");

                var container = services.BuildServiceProvider();

                var loc = container.GetRequiredService<LocalizedClass>();

                Assert.Equal("Tableau Migration SDK", loc.GetLocalizedString());
            }
        }

        public class SharedResourceKeysTests
        {

            [Fact]
            public void Error_on_bad_input()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

                var bad = localizer["ThisStringDoesNotExist"];
                Assert.True(bad.ResourceNotFound);
            }

            [Fact]
            public void All_keys_valid()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

                var constants = typeof(SharedResourceKeys)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .ToList();

                Assert.All(constants, (constant) =>
                {
                    // If this fails, it means the key is not in the SharedResource.resx file

                    var localized = localizer[constant.Name];
                    Assert.False(localized.ResourceNotFound);
                });
            }

            [Fact]
            public void SharedResourceKeys_and_SharedResources_are_in_sync()
            {
                var services = CreateLocalizationTestServices();
                var container = services.BuildServiceProvider();

                var localizer = container.GetRequiredService<ISharedResourcesLocalizer>();

                // Get all string constants from SharedResourceKeys
                var keyConstants = typeof(SharedResourceKeys)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => f.GetValue(null)?.ToString())
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .ToHashSet()!;

                // Verify each key from SharedResourceKeys can be resolved                
                var missingKeys = new List<string>();
                var foundKeys = new List<string>();

                foreach (var key in keyConstants)
                {
                    if (key is null)
                    {
                        continue;
                    }

                    var localized = localizer[key];
                    if (localized.ResourceNotFound)
                    {
                        missingKeys.Add(key);
                        continue;
                    }

                    foundKeys.Add(key);
                }

                // Report any keys from SharedResourceKeys that are missing in SharedResources.resx
                if (missingKeys.Count != 0)
                {
                    var missingKeysMessage = $"Keys defined in SharedResourceKeys but missing in SharedResources.resx:\n  - {string.Join("\n  - ", missingKeys)}";
                    Assert.Fail($"{nameof(SharedResourceKeys)} and SharedResources.resx are out of sync:\n\n{missingKeysMessage}");
                }


                // Verify all keys were found
                Assert.Equal(keyConstants.Count, foundKeys.Count);
            }
        }
    }
}
