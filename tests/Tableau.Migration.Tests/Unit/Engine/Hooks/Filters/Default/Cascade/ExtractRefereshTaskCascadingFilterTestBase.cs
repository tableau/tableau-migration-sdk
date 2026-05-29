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

using System;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default.Cascade;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default.Cascade
{
    public abstract class ExtractRefereshTaskCascadingFilterTestBase<TFilter, TTask, TSchedule> : CascadingFilterTestBase
        where TFilter : ExtractRefereshTaskCascadingFilterBase<TTask, TSchedule>
        where TTask : IExtractRefreshTask<TSchedule>
        where TSchedule : ISchedule
    {
        [Fact]
        public void DataSourceCascades()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TTask>>();
            ctx.SourceItem.ContentType = ExtractRefreshContentType.DataSource;
            CascadeSkipEntries[ctx.SourceItem.Content.Location] = true;

            f.Filter(ctx);

            Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
            AssertContentReferenceTypeSearched<IDataSource>();
        }

        [Fact]
        public void WorkbookCascades()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TTask>>();
            ctx.SourceItem.ContentType = ExtractRefreshContentType.Workbook;
            CascadeSkipEntries[ctx.SourceItem.Content.Location] = true;

            f.Filter(ctx);

            Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
            AssertContentReferenceTypeSearched<IWorkbook>();
        }

        [Fact]
        public void UnknownContentReferenceDoesNotCascade()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TTask>>();
            ctx.SourceItem.ContentType = ExtractRefreshContentType.Unknown;

            f.Filter(ctx);

            Assert.Equal(FilterStatus.Migrate, ctx.Status);
            MockManifestPartition.VerifyNoOtherCalls();
        }

        [Fact]
        public void InvalidContentReferenceType()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TTask>>();
            ctx.SourceItem.ContentType = (ExtractRefreshContentType)int.MaxValue;

            Assert.Throws<ArgumentException>(() => f.Filter(ctx));
        }
    }
}
