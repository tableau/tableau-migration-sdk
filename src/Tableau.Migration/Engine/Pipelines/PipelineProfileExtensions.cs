using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Pipelines
{
    internal static class PipelineProfileExtensions
    {
        public static ImmutableArray<MigrationPipelineContentType> GetSupportedContentTypes(this PipelineProfile profile)
            => profile switch
            {
                PipelineProfile.ServerToCloud => ServerToCloudMigrationPipeline.ContentTypes,
                _ => throw new ArgumentException($"The profile {profile} is not supported", nameof(profile)),
            };
    }
}
