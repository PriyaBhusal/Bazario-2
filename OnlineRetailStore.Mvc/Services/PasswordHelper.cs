using System.Security.Cryptography;
using System.Text;

namespace OnlineRetailStore.Mvc.Services
{
    /// <summary>Matches the legacy app's simple SHA256 (lowercase hex) password hashing so
    /// existing rows in the users table keep working unchanged.</summary>
    public static class PasswordHelper
    {
        public static string HashSha256(string plain)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain ?? string.Empty));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
