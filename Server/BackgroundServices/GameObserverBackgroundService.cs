using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Models.Dtos;

namespace Server.BackgroundServices
{
    public class GameObserverBackgroundService : BackgroundService
    {

        private readonly IGameService _gameService;
        private readonly IHubContext<GameHub> _gameHubContext;

        public GameObserverBackgroundService(IGameService gameService, IHubContext<GameHub> gameHubContext)
        {
            _gameService = gameService;
            _gameHubContext = gameHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(2000); //
            }
        }
    }
}
