using ShotgunClassLibrary.Dtos;

namespace Server.Services.Interfaces
{
    public interface IRsaService
    {
        public RsaPublicKeyDto GetPublicKey();
        public string Decrypt(string message);
    }
}
