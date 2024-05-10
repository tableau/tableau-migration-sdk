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

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class TableauCloudUsernameMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private string MailDomain { get; set; } = string.Empty;

            private bool UseExistingEmail { get; set; } = true;

            private readonly CancellationToken _cancel = new();

            protected TableauCloudUsernameMapping BuildMapping()
            {
                var opts = new TableauCloudUsernameMappingOptions
                {
                    MailDomain = MailDomain,
                    UseExistingEmail = UseExistingEmail
                };

                var mockOptionsProvider = Create<Mock<IMigrationPlanOptionsProvider<TableauCloudUsernameMappingOptions>>>();
                mockOptionsProvider.Setup(x => x.Get()).Returns(opts);

                return new TableauCloudUsernameMapping(mockOptionsProvider.Object);
            }

            [Fact]
            public async Task ExistingEmailAsync()
            {
                MailDomain = "test.com";
                var mapping = BuildMapping();

                var ctx = Create<ContentMappingContext<IUser>>();
                ctx.ContentItem.Email = "email@fake.com";

                var result = await mapping.ExecuteAsync(ctx, _cancel);

                Assert.NotSame(ctx, result);

                Assert.NotNull(result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(ctx.MappedLocation.Parent().Append("email@fake.com"), result.MappedLocation);
            }

            [Fact]
            public async Task SkipExistingEmailAsync()
            {
                MailDomain = "test.com";
                UseExistingEmail = false;
                var mapping = BuildMapping();

                var ctx = Create<ContentMappingContext<IUser>>();
                ctx.ContentItem.Email = "email@fake.com";

                var result = await mapping.ExecuteAsync(ctx, _cancel);

                Assert.NotSame(ctx, result);

                Assert.NotNull(result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(ctx.MappedLocation.Parent().Append($"{ctx.MappedLocation.Name}@test.com"), result.MappedLocation);
            }

            [Fact]
            public async Task GeneratedUsernameAsync()
            {
                MailDomain = "test.com";
                var mapping = BuildMapping();

                var ctx = Create<ContentMappingContext<IUser>>();
                ctx.ContentItem.Email = string.Empty;

                var result = await mapping.ExecuteAsync(ctx, _cancel);

                Assert.NotSame(ctx, result);

                Assert.NotNull(result);
                Assert.Same(ctx.ContentItem, result.ContentItem);
                Assert.Equal(ctx.MappedLocation.Parent().Append($"{ctx.MappedLocation.Name}@test.com"), result.MappedLocation);
            }
        }
    }
}
