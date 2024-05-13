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
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ValidationExtensionsTests
    {
        private class TestValidationType
        {
            [Required]
            public string? Name { get; set; }

            [Range(0, int.MaxValue)]
            public int Id { get; set; }
        }

        public class ValidateSimpleProperties
        {
            [Fact]
            public void ValidReturnsSuccess()
            {
                var valid = new TestValidationType
                {
                    Name = "test",
                    Id = 47
                };

                var result = valid.ValidateSimpleProperties();

                result.AssertSuccess();
            }

            [Fact]
            public void SingleValidationError()
            {
                var valid = new TestValidationType
                {
                    Name = null,
                    Id = 47
                };

                var result = valid.ValidateSimpleProperties();

                result.AssertFailure();
                var error = Assert.Single(result.Errors);
                var validationError = Assert.IsType<ValidationException>(error);
            }

            [Fact]
            public void MultipleValidationErrors()
            {
                var valid = new TestValidationType
                {
                    Name = null,
                    Id = -47
                };

                var result = valid.ValidateSimpleProperties();

                result.AssertFailure();
                Assert.Equal(2, result.Errors.Count);
            }
        }
    }
}
