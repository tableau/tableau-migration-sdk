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
