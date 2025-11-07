using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace InmobiliariaAPI.Services
{
    public class HashService
    {
        public static string HashClave(string salt, string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: System.Text.Encoding.ASCII.GetBytes(salt),
              prf: KeyDerivationPrf.HMACSHA1,
              iterationCount: 1000,
              numBytesRequested: 256 / 8));
        }
    }
}
