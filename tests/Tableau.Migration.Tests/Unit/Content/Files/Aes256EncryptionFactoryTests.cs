using System.Security.Cryptography;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class Aes256EncryptionFactoryTests
    {
        public class Create
        {
            [Fact]
            public void CreatesAes256()
            {
                using var aes = new Aes256EncryptionFactory().Create();

                Assert.IsAssignableFrom<Aes>(aes);
                Assert.Equal(256, aes.KeySize);
            }
        }
    }
}
