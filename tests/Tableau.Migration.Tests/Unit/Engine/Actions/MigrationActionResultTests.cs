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
using Tableau.Migration.Engine.Actions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Actions
{
    public class MigrationActionResultTests
    {
        public class Succeeded
        {
            [Fact]
            public void DefaultNextAction()
            {
                var r = MigrationActionResult.Succeeded();

                Assert.True(r.Success);
                Assert.True(r.PerformNextAction);
                Assert.Empty(r.Errors);
            }

            [Fact]
            public void ExplicitNextAction()
            {
                var r = MigrationActionResult.Succeeded(false);

                Assert.True(r.Success);
                Assert.False(r.PerformNextAction);
                Assert.Empty(r.Errors);
            }
        }

        public class Failed
        {
            public class SingleException
            {
                [Fact]
                public void DefaultNextAction()
                {
                    var error = new Exception();
                    var r = MigrationActionResult.Failed(error);

                    Assert.False(r.Success);
                    Assert.True(r.PerformNextAction);
                    Assert.Same(error, Assert.Single(r.Errors));
                }

                [Fact]
                public void ExplicitNextAction()
                {
                    var error = new Exception();
                    var r = MigrationActionResult.Failed(error, false);

                    Assert.False(r.Success);
                    Assert.False(r.PerformNextAction);
                    Assert.Same(error, Assert.Single(r.Errors));
                }
            }

            public class ExceptionCollection
            {
                [Fact]
                public void DefaultNextAction()
                {
                    var errors = new[] { new Exception() };
                    var r = MigrationActionResult.Failed(errors);

                    Assert.False(r.Success);
                    Assert.True(r.PerformNextAction);
                    Assert.Equal(errors, r.Errors);
                }

                [Fact]
                public void ExplicitNextAction()
                {
                    var errors = new[] { new Exception() };
                    var r = MigrationActionResult.Failed(errors, false);

                    Assert.False(r.Success);
                    Assert.False(r.PerformNextAction);
                    Assert.Equal(errors, r.Errors);
                }
            }
        }

        public class FromResult
        {
            [Fact]
            public void FromSuccess()
            {
                var success = Result.Succeeded();

                var result = MigrationActionResult.FromResult(success);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);
            }

            [Fact]
            public void FromFailure()
            {
                var failure = Result.Failed(new[] { new Exception(), new Exception() });

                var result = MigrationActionResult.FromResult(failure);

                result.AssertFailure();
                Assert.Equal(failure.Errors, result.Errors);
                Assert.False(result.PerformNextAction);
            }

            [Fact]
            public void OverrideSuccess()
            {
                var success = Result.Succeeded();

                var result = MigrationActionResult.FromResult(success, false);

                result.AssertSuccess();
                Assert.False(result.PerformNextAction);
            }

            [Fact]
            public void OverrideFailure()
            {
                var failure = Result.Failed(new[] { new Exception(), new Exception() });

                var result = MigrationActionResult.FromResult(failure, true);

                result.AssertFailure();
                Assert.Equal(failure.Errors, result.Errors);
                Assert.True(result.PerformNextAction);
            }
        }

        public class ForNextAction
        {
            [Fact]
            public void ModifiedNextAction()
            {
                var r1 = MigrationActionResult.Succeeded();
                var r2 = r1.ForNextAction(false);

                Assert.Equal(r1.Success, r2.Success);
                Assert.Equal(r1.Errors, r2.Errors);
                Assert.NotEqual(r1.PerformNextAction, r2.PerformNextAction);
            }
        }
    }
}
