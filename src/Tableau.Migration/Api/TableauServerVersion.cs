namespace Tableau.Migration.Api
{
    /// <summary>
    /// Object containing version information for a Tableau server.
    /// </summary>
    /// <param name="RestApiVersion">The server's REST API version.</param>
    /// <param name="ProductVersion">The server's product version.</param>
    /// <param name="BuildVersion">The server's build version.</param>
    public readonly record struct TableauServerVersion(string RestApiVersion, string ProductVersion, string BuildVersion)
    { }
}
