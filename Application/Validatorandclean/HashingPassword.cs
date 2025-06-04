using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class HashingPassword

    {
        private const int SaltSize = 32; // 256 bits
        private const int HashSize = 64; // 512 bits
        private const int Iterations = 100000; // Recommended minimum

        public static string HashPassword(string password)
        {
            // Generate salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Generate hash
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA512))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            // Combine salt + hash + iteration
            var hashBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);

            // Return as Base64
            return $"{Iterations}.{Convert.ToBase64String(hashBytes)}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                var parts = hashedPassword.Split('.');
                if (parts.Length != 2)
                    return false;

                int iterations = int.Parse(parts[0]);
                byte[] hashBytes = Convert.FromBase64String(parts[1]);

                byte[] salt = new byte[SaltSize];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

                byte[] originalHash = new byte[HashSize];
                Buffer.BlockCopy(hashBytes, SaltSize, originalHash, 0, HashSize);

                byte[] testHash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
                {
                    testHash = pbkdf2.GetBytes(HashSize);
                }

                // Constant time comparison
                return AreHashesEqual(originalHash, testHash);
            }
            catch
            {
                return false;
            }
        }

        private static bool AreHashesEqual(byte[] hash1, byte[] hash2)
        {
            uint diff = (uint)hash1.Length ^ (uint)hash2.Length;
            for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
                diff |= (uint)(hash1[i] ^ hash2[i]);
            return diff == 0;
        }
    }


}