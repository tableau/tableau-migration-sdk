namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item that has an extract.
    /// </summary>
    public interface IExtractContent
    {
        /// <summary>
        /// Gets or sets whether or not extracts are encrypted.
        /// </summary>
        bool EncryptExtracts { get; set; }
    }
}
