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
using System.Collections.Immutable;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class TestPipeline : MigrationPipelineBase
    {
        public int BuildPipelineCalls { get; private set; }

        public ImmutableArray<TestAction> TestPipelineActions { get; private set; }

        public TestPipeline(IServiceProvider services, IConfigReader configReader)
            : base(services, configReader)
        {
            TestPipelineActions = new[]
            {
                    base.CreateAction<TestAction>(),
                    base.CreateAction<TestAction>(),
                    base.CreateAction<TestAction>()
            }.ToImmutableArray();
        }

        new public TAction CreateAction<TAction>()
            where TAction : IMigrationAction
            => base.CreateAction<TAction>();

        new public IMigrateContentAction<TContent> CreateMigrateContentAction<TContent>()
            where TContent : class, IContentReference
            => base.CreateMigrateContentAction<TContent>();

        protected override IEnumerable<IMigrationAction> BuildPipeline()
        {
            BuildPipelineCalls++;

            return TestPipelineActions;
        }
    }
}
