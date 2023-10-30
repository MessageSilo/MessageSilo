using Azure;
using Azure.Data.Tables;
using MessageSilo.Shared.Enums;
using System.Security.Cryptography;
using System.Text;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class Entity: IComparable<Entity>
    {
        [Id(0)]
        public string UserId { get; set; }

        [Id(1)]
        public string Name { get; set; }

        [Id(2)]
        public EntityKind Kind { get; set; }

        [YamlIgnore]
        public string Id => $"{UserId}|{Name}";

        [Id(3)]
        private byte[] IV =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };

        public int CompareTo(Entity? other)
        {
            var result = this.Id.CompareTo(other?.Id);

            if (result == 0)
                result = this.Kind.CompareTo(other?.Kind);

            return result;
        }

        protected async Task<string> encryptAsync(string clearText, string passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = deriveKeyFromPassword(passphrase);
            aes.IV = IV;

            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
            await cryptoStream.FlushFinalBlockAsync();

            return Convert.ToBase64String(output.ToArray());
        }

        protected async Task<string> decryptAsync(string encrypted, string passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = deriveKeyFromPassword(passphrase);
            aes.IV = IV;

            using MemoryStream input = new(Convert.FromBase64String(encrypted));
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);

            return Encoding.Unicode.GetString(output.ToArray());
        }

        private byte[] deriveKeyFromPassword(string password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            var hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }
    }
}
