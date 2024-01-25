namespace Tableau.Migration.Interop
{
    internal class PythonUserAgentSuffixProvider : IUserAgentSuffixProvider
    {
        /// <summary>
        /// Python user agent suffix
        /// </summary>
        public string UserAgentSuffix { get; init; } = Constants.USER_AGENT_PYTHON_SUFFIX;
    }
}
