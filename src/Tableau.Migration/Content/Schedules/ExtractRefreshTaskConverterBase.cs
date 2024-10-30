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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules
{
    internal abstract class ExtractRefreshTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule> : IExtractRefreshTaskConverter<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>
        where TSourceTask : IExtractRefreshTask<TSourceSchedule>
        where TSourceSchedule : ISchedule
        where TTargetTask : IExtractRefreshTask<TTargetSchedule>
        where TTargetSchedule : ISchedule
    {
        protected ILogger<ExtractRefreshTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>> Logger { get; }
        protected ISharedResourcesLocalizer Localizer { get; }

        protected ExtractRefreshTaskConverterBase(
            ILogger<ExtractRefreshTaskConverterBase<TSourceTask, TSourceSchedule, TTargetTask, TTargetSchedule>> logger,
            ISharedResourcesLocalizer localizer)
        {
            Logger = logger;
            Localizer = localizer;
        }

        /// <inheritdoc />
        public TTargetTask Convert(TSourceTask source)
        {
            var targetSchedule = ConvertSchedule(source.Schedule);
            return ConvertExtractRefreshTask(source, targetSchedule);
        }

        protected abstract TTargetSchedule ConvertSchedule(TSourceSchedule sourceSchedule);

        protected abstract TTargetTask ConvertExtractRefreshTask(TSourceTask source, TTargetSchedule targetSchedule);
    }
}
