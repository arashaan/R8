using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

using System;
using System.Linq;
using System.Text;

namespace R8.Lib
{
    public static class Encryption
    {
        /// <summary>
        /// An enumerator constant to choose which type of cipher should be used.
        /// </summary>
        public enum CipherMode
        {
            Encryption = 0,
            Decryption = 1
        }

        public static readonly byte[] Key = { 115, 6, 90, 196, 167, 224, 85, 212, 19, 51, 249, 142, 190, 12, 116, 158 };
        private static readonly byte[] Iv = { 179, 18, 225, 48, 218, 84, 112, 223, 125, 137, 126, 36, 98, 232, 231, 124 };

        /// <summary>
        /// Returns an decrypted <see cref="string"/> of given data.
        /// </summary>
        /// <param name="string">A <see cref="string"/> value to be performed as encrypted data.</param>
        /// <param name="sharedKey"></param>
        /// <param name="iv"></param>
        /// <returns>An decrypted <see cref="string"/> based on specific strategy.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="object"/> to <see cref="string"/>.</remarks>
        public static string Decipherize(this string @string, byte[] sharedKey, byte[] iv)
        {
            if (@string == null) throw new ArgumentNullException(nameof(@string));
            if (sharedKey == null || !sharedKey.Any()) throw new ArgumentNullException(nameof(sharedKey));
            if (iv == null || !iv.Any()) throw new ArgumentNullException(nameof(iv));

            var kdf2BytesGenerator = new Kdf2BytesGenerator(new Sha1Digest());
            var kdfParameters = new KdfParameters(sharedKey, iv);
            kdf2BytesGenerator.Init(kdfParameters);

            var key = new byte[16];
            kdf2BytesGenerator.GenerateBytes(key, 0, key.Length);

            var aesLightEngine = new AesLightEngine();
            var cbcBlockCipher = new CbcBlockCipher(aesLightEngine);
            var paddedBufferedBlockCipher = new PaddedBufferedBlockCipher(cbcBlockCipher, new Pkcs7Padding());
            var keyParameter = new KeyParameter(key);
            var parametersWithIv = new ParametersWithIV(keyParameter, iv, 0, iv.Length);

            var targetBytes = Convert.FromBase64String(@string);

            paddedBufferedBlockCipher.Init(false, parametersWithIv);
            var output = new byte[paddedBufferedBlockCipher.GetOutputSize(targetBytes.Length)];
            var length = paddedBufferedBlockCipher.ProcessBytes(targetBytes, output, 0);
            paddedBufferedBlockCipher.DoFinal(output, length);

            return Encoding.UTF8.GetString(output.Where(@byte => @byte != 0).ToArray());
        }

        /// <summary>
        /// Returns an encrypted <see cref="string"/> of given data.
        /// </summary>
        /// <param name="string">A <see cref="string"/> value to be performed as non-encrypted data.</param>
        /// <param name="sharedKey"></param>
        /// <param name="iv"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An encrypted <see cref="string"/>.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="object"/> to <see cref="string"/>.</remarks>
        public static string Cipherize(this string @string, byte[] sharedKey, byte[] iv)
        {
            if (@string == null) throw new ArgumentNullException(nameof(@string));
            if (sharedKey == null || !sharedKey.Any()) throw new ArgumentNullException(nameof(sharedKey));
            if (iv == null || !iv.Any()) throw new ArgumentNullException(nameof(iv));

            var kdf2BytesGenerator = new Kdf2BytesGenerator(new Sha1Digest());
            var kdfParameters = new KdfParameters(sharedKey, iv);
            kdf2BytesGenerator.Init(kdfParameters);

            var key = new byte[16];
            kdf2BytesGenerator.GenerateBytes(key, 0, key.Length);

            var aesLightEngine = new AesLightEngine();
            var cbcBlockCipher = new CbcBlockCipher(aesLightEngine);
            var paddedBufferedBlockCipher = new PaddedBufferedBlockCipher(cbcBlockCipher, new Pkcs7Padding());
            var keyParameter = new KeyParameter(key);
            var parametersWithIv = new ParametersWithIV(keyParameter, iv, 0, iv.Length);

            var targetBytes = Encoding.UTF8.GetBytes(@string);

            paddedBufferedBlockCipher.Init(true, parametersWithIv);
            var output = new byte[paddedBufferedBlockCipher.GetOutputSize(targetBytes.Length)];
            var length = paddedBufferedBlockCipher.ProcessBytes(targetBytes, output, 0);
            paddedBufferedBlockCipher.DoFinal(output, length);

            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Encrypts or decrypts an <see cref="string" /> value.
        /// </summary>
        /// <param name="string">A <see cref="string"/> value that performed to encryption or decryption.</param>
        /// <param name="mode">Which type of Ciphering should be used.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An processed <see cref="string"/> due to given <see cref="R8.Lib.Encryption.CipherMode"/>.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="object"/> to <see cref="string"/>.</remarks>
        public static string Cipher(this string @string, CipherMode mode = CipherMode.Encryption)
        {
            return mode == CipherMode.Encryption
                ? @string.Cipherize(Key, Iv)
                : @string.Decipherize(Key, Iv);
        }
    }
}