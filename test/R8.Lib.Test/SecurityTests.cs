using Xunit;

namespace R8.Lib.Test
{
    public class SecurityTests
    {
        [Fact]
        public void CallCipher2()
        {
            const string plainText = "Arash";
            var encrypted = Security.Cipher(plainText);

            var decrypted = Security.Cipher(encrypted, Security.CipherMode.Decryption);
            Assert.Equal(decrypted, plainText);
        }
    }
}