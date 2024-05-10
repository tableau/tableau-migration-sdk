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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Tableau.Migration.Resources
{
    /// <summary>
    /// Interface for an object that can provide localized string from shared Tableau Migration SDK resources.
    /// </summary>
    /// <remarks>
    /// This interface is used so that <see cref="IStringLocalizer"/>s can be configured to use library resource files,
    /// regardless of where resource paths are configured by the user application through 
    /// <see cref="LocalizationServiceCollectionExtensions.AddLocalization(IServiceCollection, System.Action{LocalizationOptions})"/>.
    /// 
    /// This interface is registered automatically through <see cref="IServiceCollectionExtensions.AddTableauMigrationSdk(IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration?)"/>
    /// and is not intended for use outside of the Tableau Migration SDK.
    /// </remarks>
    public interface ISharedResourcesLocalizer : IStringLocalizer
    { }
}
