using System;
using System.Linq;
using System.Text;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace R8.Lib.Encryption
{
    public static class Cyptology
    {
        public static readonly byte[] Key = { 115, 6, 90, 196, 167, 224, 85, 212, 19, 51, 249, 142, 190, 12, 116, 158 };
        private static readonly byte[] Iv = { 179, 18, 225, 48, 218, 84, 112, 223, 125, 137, 126, 36, 98, 232, 231, 124 };

        /// <summary>
        /// Encrypts or decrypts an <see cref="string" /> value.
        /// </summary>
        /// <param name="data">A <see cref="string"/> value that performed to encryption or decryption.</param>
        /// <param name="mode">Which type of Ciphering should be used.</param>
        /// <returns>An processed <see cref="string"/> due to given <see cref="CipherMode"/>.</returns>
        /// <remarks>For non-string values and objects, you need to convert <see cref="object"/> to <see cref="string"/>.</remarks>
        public static string Cipher(this string data, CipherMode mode = CipherMode.Encryption)
        {
            var kdf2BytesGenerator = new Kdf2BytesGenerator(new Sha1Digest());
            var kdfParameters = new KdfParameters(Key, Iv);
            kdf2BytesGenerator.Init(kdfParameters);

            var key = new byte[16];
            kdf2BytesGenerator.GenerateBytes(key, 0, key.Length);

            var engine = new AesLightEngine();
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var parameter = new KeyParameter(key);
            var parameterWithIv = new ParametersWithIV(parameter, Iv, 0, 16);

            var targetBytes = mode == CipherMode.Decryption
                ? Convert.FromBase64String(data)
                : Encoding.UTF8.GetBytes(data);

            cipher.Init(mode == CipherMode.Encryption, parameterWithIv);
            var output = new byte[cipher.GetOutputSize(targetBytes.Length)];
            var length = cipher.ProcessBytes(targetBytes, output, 0);
            cipher.DoFinal(output, length);

            return mode == CipherMode.Encryption
                ? Convert.ToBase64String(output)
                : Encoding.UTF8.GetString(output.Where(@byte => @byte != 0).ToArray());
        }
    }
}