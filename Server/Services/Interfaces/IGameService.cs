using ShotgunClassLibrary.Classes.Game;
using ShotgunClassLibrary.Models.Dtos;
using Server.Models.Schemas;
using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Schemas;

namespace Server.Services.Interfaces
{
    public interface IGameService
    {
        public Task<ResultDto> CreateAsync(string username, GameConfig gameConfig);
        public Task<ResultDto> JoinAsync(string username, Guid gameId);
        public Task<ResultDto> GetQueueAsync();
        public Task<ResultDto> ProcessActionAsync(string username, PlayerActionRequest playerActionRequest);
        public Task<Guid> GetPlayerGameIdAsync(string username);
    }
}
