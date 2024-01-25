namespace Tableau.Migration
{
    /// <summary>
    /// Enumeration of the various supported migration pipeline profiles.
    /// </summary>
    public enum PipelineProfile
    {
        //Custom = 0, //Uncomment when custom pipelines are supported.

        /// <summary>
        /// The pipeline to bulk migrate content from a Tableau Server site to a Tableau Cloud site.
        /// </summary>
        ServerToCloud = 1
    }
}
