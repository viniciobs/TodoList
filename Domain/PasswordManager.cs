using Isopoh.Cryptography.Argon2;

namespace Domains
{
    public static class PasswordManager
    {
        internal static string Hash(string entry)
        {
            return Argon2.Hash(entry);
        }

        public static bool AreEqual(string hashed, string plainText)
        {
            return Argon2.Verify(hashed, plainText);
        }
    }
}