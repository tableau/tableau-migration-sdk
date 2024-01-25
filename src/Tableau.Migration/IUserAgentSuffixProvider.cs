namespace Tableau.Migration
{
    /// <summary>
    /// Interface for an object that builds the user agent suffix
    /// </summary>
    internal interface IUserAgentSuffixProvider
    {
        public string UserAgentSuffix { get; init; }
    }
}
