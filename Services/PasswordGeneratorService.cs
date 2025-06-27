using System;
using System.Linq;
using System.Security.Cryptography;

namespace PasswordVault.Services
{
    public class PasswordGeneratorService
    {
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumberChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        public static string GeneratePassword(
            int length = 16,
            bool includeLowercase = true,
            bool includeUppercase = true,
            bool includeNumbers = true,
            bool includeSpecial = true)
        {
            var chars = "";
            if (includeLowercase) chars += LowercaseChars;
            if (includeUppercase) chars += UppercaseChars;
            if (includeNumbers) chars += NumberChars;
            if (includeSpecial) chars += SpecialChars;

            if (string.IsNullOrEmpty(chars))
                throw new ArgumentException("At least one character set must be selected.");

            var result = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            // Ensure at least one character from each selected set
            var currentIndex = 0;
            if (includeLowercase)
                result[currentIndex++] = LowercaseChars[RandomNumberGenerator.GetInt32(LowercaseChars.Length)];
            if (includeUppercase)
                result[currentIndex++] = UppercaseChars[RandomNumberGenerator.GetInt32(UppercaseChars.Length)];
            if (includeNumbers)
                result[currentIndex++] = NumberChars[RandomNumberGenerator.GetInt32(NumberChars.Length)];
            if (includeSpecial)
                result[currentIndex++] = SpecialChars[RandomNumberGenerator.GetInt32(SpecialChars.Length)];

            // Shuffle the result
            return new string(result.OrderBy(x => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
        }
    }
} 