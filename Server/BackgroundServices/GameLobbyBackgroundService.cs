using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server.Hubs;
using ShotgunClassLibrary.Models.Dtos;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Dtos;

namespace Server.BackgroundServices
{
    public class GameLobbyBackgroundService : BackgroundService
    {

        private readonly IGameService _gameService;
        private readonly IHubContext<GameLobbyHub> _gameLobbyHubContext;

        public GameLobbyBackgroundService(IGameService gameService, IHubContext<GameLobbyHub> gameLobbyHubContext)
        {
            _gameService = gameService;
            _gameLobbyHubContext = gameLobbyHubContext;
        }

        /*
            Returns joinable games to every connected client every x seconds
         */
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ResultDto result = await _gameService.GetQueueAsync();

                await _gameLobbyHubContext.Clients.All.SendAsync("GameListUpdateAsync", result, result.Data);

                await Task.Delay(2000);
            }
        }
    }
}
