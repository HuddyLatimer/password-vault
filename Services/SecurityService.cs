using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace PasswordVault.Services
{
    public class SecurityService
    {
        private readonly string _key;
        private const string SALT = "PasswordVault2023";

        public SecurityService(string masterPassword)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(
                masterPassword, 
                Encoding.UTF8.GetBytes(SALT), 
                310000, // Recommended iteration count for 2023
                HashAlgorithmName.SHA256);
            _key = Convert.ToBase64String(deriveBytes.GetBytes(32));
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_key);
            aes.GenerateIV();

            using var msEncrypt = new MemoryStream();
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using (var csEncrypt = new CryptoStream(msEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);

            using var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            aes.Key = Convert.FromBase64String(_key);
            aes.IV = iv;

            using var csDecrypt = new CryptoStream(msDecrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }

        public static string GetMacAddress()
        {
            try
            {
                using var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();
                var macAddress = moc
                    .Cast<ManagementObject>()
                    .Where(mo => (bool)mo["IPEnabled"])
                    .Select(mo => mo["MacAddress"].ToString())
                    .FirstOrDefault();

                return macAddress?.Replace(":", "-") ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + SALT));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 