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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Services
{
    /// <summary>
    /// Default <see cref="IMigrationServiceBuilder"/> implementation.
    /// </summary>
    internal sealed class MigrationServiceBuilder : IMigrationServiceBuilder
    {
        internal static readonly MigrationServiceBuilder Empty = new(ImmutableArray<Type>.Empty);

        private Dictionary<Type, MigrationServiceFactory> _services;

        public MigrationServiceBuilder(IImmutableList<Type> supportedServices)
        {
            _services = new();
            SupportedServices = supportedServices;
        }

        /// <inheritdoc />
        public IImmutableList<Type> SupportedServices { get; }

        /// <inheritdoc />
        public MigrationServiceFactory? GetServiceFactory<TService>()
            => GetServiceFactory(typeof(TService));

        /// <inheritdoc />
        public MigrationServiceFactory? GetServiceFactory(Type serviceType)
        {
            if(_services.TryGetValue(serviceType, out var factory))
            {
                return factory;
            }

            return null;
        }

        /// <inheritdoc />
        public TService GetService<TService>(IServiceProvider services)
            where TService : notnull
        {
            //Match exact type first, but allow an open generic to handle closed generic type.
            var serviceFactory = GetServiceFactory<TService>();
            if(serviceFactory is null && typeof(TService).IsGenericType)
            {
                var openGeneric = typeof(TService).GetGenericTypeDefinition();
                serviceFactory = GetServiceFactory(openGeneric);
            }

            if (serviceFactory is not null)
            {
                return (TService)serviceFactory(new(services, typeof(TService)));
            }

            return services.GetRequiredService<TService>();
        }

        /// <inheritdoc />
        public IMigrationServiceBuilder Remove<TService>()
            => Remove(typeof(TService));

        /// <inheritdoc />
        public IMigrationServiceBuilder Remove(Type service)
        {
            _services.Remove(service);
            return this;
        }

        /// <inheritdoc />
        public IMigrationServiceBuilder Set<TService>(MigrationServiceFactory factory)
            => Set(typeof(TService), factory);

        /// <inheritdoc />
        public IMigrationServiceBuilder Set(Type service, MigrationServiceFactory factory)
        {
            _services[service] = factory;
            return this;
        }
    }
}
