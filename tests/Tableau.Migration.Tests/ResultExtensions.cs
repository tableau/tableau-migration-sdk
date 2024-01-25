using Xunit;

namespace Tableau.Migration.Tests
{
    internal static class ResultExtensions
    {
        public static void AssertSuccess(this IResult result)
        {
            Assert.Empty(result.Errors);
            Assert.True(result.Success);
        }

        public static void AssertFailure(this IResult result)
        {
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
        }
    }
}
