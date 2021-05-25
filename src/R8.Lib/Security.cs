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
    public static class Security
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
        public static readonly byte[] Iv = { 179, 18, 225, 48, 218, 84, 112, 223, 125, 137, 126, 36, 98, 232, 231, 124 };

        /// <summary>
        /// Encrypts or decrypts an <see cref="string" /> value.
        /// </summary>
        /// <param name="str">A <see cref="string"/> value that performed to encryption or decryption.</param>
        /// <param name="sharedKey"></param>
        /// <param name="iv"></param>
        /// <param name="mode">Which type of Ciphering should be used.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An processed <see cref="string"/> due to given <see cref="object"/>.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="string"/> to <see cref="CipherMode"/>.</remarks>
        public static string Cipher(string str, byte[] sharedKey, byte[] iv, CipherMode mode = CipherMode.Encryption)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
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

            var targetBytes = mode == CipherMode.Encryption
                ? Encoding.UTF8.GetBytes(str)
                : Convert.FromBase64String(str);

            paddedBufferedBlockCipher.Init(mode == CipherMode.Encryption, parametersWithIv);
            var output = new byte[paddedBufferedBlockCipher.GetOutputSize(targetBytes.Length)];
            var length = paddedBufferedBlockCipher.ProcessBytes(targetBytes, output, 0);
            paddedBufferedBlockCipher.DoFinal(output, length);

            return mode == CipherMode.Encryption
                ? Convert.ToBase64String(output)
                : Encoding.UTF8.GetString(output.Where(@byte => @byte != 0).ToArray());
        }

        /// <summary>
        /// Encrypts or decrypts an <see cref="string" /> value.
        /// </summary>
        /// <param name="str">A <see cref="string"/> value that performed to encryption or decryption.</param>
        /// <param name="mode">Which type of Ciphering should be used.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An processed <see cref="string"/> due to given <see cref="CipherMode"/>.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="object"/> to <see cref="string"/>.</remarks>
        public static string Cipher(string str, CipherMode mode = CipherMode.Encryption)
        {
            return Cipher(str, Key, Iv, mode);
        }
    }
}