using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace coffre_fort2.Models
{
    public static class PasswordManager
    {
        private static readonly string PasswordFilePath = "password.txt";

        public static bool IsPasswordDefined()
        {
            return File.Exists(PasswordFilePath);
        }

        public static void SavePassword(string password)
        {
            var hash = HashPassword(password);
            File.WriteAllText(PasswordFilePath, hash);
        }

        public static bool VerifyPassword(string password)
        {
            if (!IsPasswordDefined())
                return false;

            var savedHash = File.ReadAllText(PasswordFilePath);
            var inputHash = HashPassword(password);
            return savedHash == inputHash;
        }

        private static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
