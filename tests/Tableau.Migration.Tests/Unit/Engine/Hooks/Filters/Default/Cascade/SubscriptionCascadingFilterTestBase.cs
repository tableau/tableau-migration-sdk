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

using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default.Cascade;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default.Cascade
{
    public abstract class SubscriptionCascadingFilterTestBase<TFilter, TSubscription, TSchedule> : CascadingFilterTestBase
        where TFilter : SubscriptionCascadingFilterBase<TSubscription, TSchedule>
        where TSubscription : ISubscription<TSchedule>
        where TSchedule : ISchedule
    {
        [Fact]
        public void ViewCascades()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TSubscription>>();
            ctx.SourceItem.Content.Type = "view";
            CascadeSkipEntries[ctx.SourceItem.Content.Location.Parent()] = true;

            f.Filter(ctx);

            Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
            AssertContentReferenceTypeSearched<IWorkbook>();
        }

        [Fact]
        public void WorkbookCascades()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TSubscription>>();
            ctx.SourceItem.Content.Type = "workbook";
            CascadeSkipEntries[ctx.SourceItem.Content.Location] = true;

            f.Filter(ctx);

            Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
            AssertContentReferenceTypeSearched<IWorkbook>();
        }

        [Fact]
        public void UnknownContentReferenceDoesNotCascade()
        {
            var f = Create<TFilter>();

            var ctx = Create<ContentFilterContextItem<TSubscription>>();
            ctx.SourceItem.Content.Type = "notathing";

            f.Filter(ctx);

            Assert.Equal(FilterStatus.Migrate, ctx.Status);
        }
    }
}
