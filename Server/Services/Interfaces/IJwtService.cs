using Server.Models.Entities;

namespace Server.Services.Interfaces
{
    public interface IJwtService
    {
        public Task<string> GenerateAsync(UserEntity user);
    }
}
