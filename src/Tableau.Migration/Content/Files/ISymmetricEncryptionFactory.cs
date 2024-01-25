using System.Security.Cryptography;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can create objects for 
    /// symetric encryption.
    /// </summary>
    public interface ISymmetricEncryptionFactory
    {
        /// <summary>
        /// Creates a new <see cref="SymmetricAlgorithm"/> object for encryption or decryption.
        /// </summary>
        /// <returns>The created encryption object.</returns>
        SymmetricAlgorithm Create();
    }
}
