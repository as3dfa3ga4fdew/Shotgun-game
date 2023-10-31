using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShotgunClassLibrary.Helpers
{
    public class RsaHandler
    {
        /*
            Encrypts a string using rsa with a provided public key
         */
        public static string Encrypt(string publicKey, string message)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportFromPem(publicKey);
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(message);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
    }
}
