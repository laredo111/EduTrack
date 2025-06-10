using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordHashTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter PassWord: ");
            string password = Console.ReadLine();

            string salt = GenerateSalt();
            string hash = HashPassword(password, salt);

            Console.WriteLine($"\nSalt: {salt}");
            Console.WriteLine($"PasswordHash: {hash}");

            Console.WriteLine("\nCpoy and past to db.");
            Console.ReadLine();
        }

        static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combined);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
