namespace Tableau.Migration.Net
{
    internal interface INetworkTraceRedactor
    {
        string ReplaceSensitiveData(string input);

        bool IsSensitiveMultipartContent(string? contentName);
    }
}
