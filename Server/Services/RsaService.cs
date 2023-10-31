using Newtonsoft.Json;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Classes;
using ShotgunClassLibrary.Dtos;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Server.Services
{
    public class RsaService : IRsaService
    {
        private readonly IConfiguration _configuration;

        private RSACryptoServiceProvider _rsaCryptoServiceProvider;
        private string _privateKey;
        private string _publicKey;

        public RsaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _rsaCryptoServiceProvider = new RSACryptoServiceProvider(1024);
        }

        public RsaPublicKeyDto GetPublicKey()
        {
            string publicKey = "-----BEGIN PUBLIC KEY-----";
            publicKey += "MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgGAqpkpIb0yd+RdOsHwhRrNSV79d";
            publicKey += "qj8wZXQWklllcz/aanZDNtweiQWVybkbLIVylL34FuVCHh5PrQn0RusMZjt90xf2";
            publicKey += "qEfrsB7OWkvQhARqmOSgiMoYXg4e/7Ysj281pz95sCs/p7YxE/5asGeD2lCRhcgT";
            publicKey += "CS/GOmBhjyedsYujAgMBAAE=";
            publicKey += "-----END PUBLIC KEY-----";
            return new RsaPublicKeyDto() { RsaPublicKey = publicKey };
        }
        public string Decrypt(string message)
        {
            try
            {
                string privateKey = "-----BEGIN RSA PRIVATE KEY-----";
                privateKey += "MIICWgIBAAKBgGAqpkpIb0yd+RdOsHwhRrNSV79dqj8wZXQWklllcz/aanZDNtwe";
                privateKey += "iQWVybkbLIVylL34FuVCHh5PrQn0RusMZjt90xf2qEfrsB7OWkvQhARqmOSgiMoY";
                privateKey += "Xg4e/7Ysj281pz95sCs/p7YxE/5asGeD2lCRhcgTCS/GOmBhjyedsYujAgMBAAEC";
                privateKey += "gYArqJEwHixxUzK30yCqag0H8jUmCub0owscJfcxIK6u6YD1ydQJIM/COluHbv/K";
                privateKey += "YdeHWy9By7+SsUd0wnLD2TA7+X/cQdLS17q7X0QR4vfiPmCWD5FjZa2KPx7rp28w";
                privateKey += "FSlaoXb3R3/GCye7FKMgRtFVgMJ5zHb5nlhiFD64qj3/UQJBAKGHUnruhQo1oaGt";
                privateKey += "6No7EjipT5+itF53H+Z39khtt+djjUhp5uNKqnPX6mEKQGiLAb+1fKtWRI0JrfaC";
                privateKey += "kN8DLAsCQQCYaRRPEbuzPBT+xC0BRA6cxn5Lt4COFxSFWwKCnQSceP36x3PKprxm";
                privateKey += "lX/KeqsCWAVqpCRIS604DfDnpd98H0XJAkB3Hl46/e7qioZ5vtB7LxjO5D0t0kUF";
                privateKey += "bP//a/QZkaAPaaDlCAQXFhcBevcDPRYmgmx1vhcEG4hOIhdCNSM08xflAkBBVjK1";
                privateKey += "ntWHhcc+XF0qwME+5jz4OqdqwmwyqwxNcACtD5VZr61s/8OeepJ4+9NZwuA3kjxX";
                privateKey += "ndGZSSiwNcSz0jg5AkBQrjZBhN0wR2JwQothkQJNNr4zaoPfOuBQtx75BWOe43uo";
                privateKey += "oO1nECXDpkcNWBmTKj2giN0i7Xdw+RGJnGcPCG/N";
                privateKey += "-----END RSA PRIVATE KEY-----";

                byte[] messageBytes = Convert.FromBase64String(message);
                _rsaCryptoServiceProvider.ImportFromPem(privateKey);
                byte[] decryptedData = _rsaCryptoServiceProvider.Decrypt(messageBytes, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch(Exception e)
            {
                //log
                return "";
            }
        }
    }
}
