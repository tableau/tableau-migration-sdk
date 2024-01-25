using System.Security.Cryptography;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="ISymmetricEncryptionFactory"/> implementation that use AES-256 encryption.
    /// </summary>
    public class Aes256EncryptionFactory : ISymmetricEncryptionFactory
    {
        /// <inheritdoc />
        public SymmetricAlgorithm Create()
        {
            var aes = Aes.Create();
            aes.KeySize = 256;

            return aes;
        }
    }
}
