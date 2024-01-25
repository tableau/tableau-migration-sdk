using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests
{
    public static class RestExceptionExtensions
    {
        [return: NotNullIfNotNull(nameof(error))]
        public static void AssertErrorEquals(
            this RestException exception,
            [NotNullIfNotNull(nameof(error))] Error? error)
        {
            Assert.NotNull(error);
            Assert.Equal(error, exception.ToError());
        }

        public static Error ToError(this RestException exception)
            => new()
            {
                Code = exception.Code,
                Detail = exception.Detail,
                Summary = exception.Summary,
            };
    }
}
