using System;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class PipelineProfileExtensionsTests
    {
        public class GetSupportedContentTyeps
        {
            [Fact]
            public void AllPipelineProfilesReturnResults()
            {
                foreach (var val in Enum.GetValues<PipelineProfile>())
                {
                    try
                    {
                        var contentTypes = val.GetSupportedContentTypes();

                        //Empty results are fine, just don't want to throw the default argument exception.
                    }
                    catch (Exception)
                    {
                        Assert.Fail($"Content type {val} does not have defined content type support. Add a case statement to {nameof(PipelineProfileExtensions.GetSupportedContentTypes)}.");
                    }
                }
            }
        }
    }
}
