using Server.Models.Entities;

namespace Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserEntity> CreateAsync(UserEntity user);
        public Task<UserEntity> GetAsync(string username);
        public Task<bool> ExistsAsync(string username);
        public Task UpdateAsync(UserEntity user);
    }
}
