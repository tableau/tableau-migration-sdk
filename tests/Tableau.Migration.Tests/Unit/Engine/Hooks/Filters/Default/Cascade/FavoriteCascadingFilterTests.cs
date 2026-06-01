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

using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default.Cascade;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default.Cascade
{
    public sealed class FavoriteCascadingFilterTests
    {
        public sealed class Filter : CascadingFilterTestBase
        {
            [Theory]
            [EnumData<FavoriteContentType>()]
            public void ViewCascades(FavoriteContentType contentType)
            {
                var f = Create<FavoriteCascadingFilter>();

                var mockFavorite = Freeze<Mock<IFavorite>>();
                mockFavorite.SetupGet(x => x.ContentType).Returns(contentType);

                var ctx = Create<ContentFilterContextItem<IFavorite>>();
                CascadeSkipEntries[ctx.SourceItem.Content.Location] = true;

                f.Filter(ctx);

                if(contentType is FavoriteContentType.Unknown)
                {
                    Assert.Equal(FilterStatus.Migrate, ctx.Status);
                }
                else
                {
                    Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                    AssertContentReferenceTypeSearched(contentType.ToMigrationContentType());
                } 
            }
        }
    }
}
