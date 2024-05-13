using System.Security.Cryptography;
using System.Text;

namespace DepartamentosAPI.Helpers
{
    public class Encryption
    {
        public static string StringToSHA512(string s)
        {
            using (var sha512 = SHA512.Create())
            {
                var arreglo = Encoding.UTF8.GetBytes(s);

                var hash = sha512.ComputeHash(arreglo);

                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}
