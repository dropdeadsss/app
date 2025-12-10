using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services
{
    public static class HashProvider
    {
        public static async Task<string> GetHash(string value)
        {
            using (SHA512 s512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);

                byte[] hashBytes = s512.ComputeHash(bytes);

                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }
                return hashString.ToString();
            }
        }
    }
}
