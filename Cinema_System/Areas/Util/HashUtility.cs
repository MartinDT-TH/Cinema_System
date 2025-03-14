using System.Security.Cryptography;
using System.Text;

namespace Cinema_System.Areas.Util
{
    public static class HashUtility
    {
        public static string GenerateSHA256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

}
