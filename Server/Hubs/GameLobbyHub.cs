using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ShotgunClassLibrary.Models.Dtos;
using Server.Services.Interfaces;
using Server.Helpers.Extensions;
using Server.Models.Schemas;
using ShotgunClassLibrary.Classes.Game;
using ShotgunClassLibrary.Dtos;

namespace Server.Hubs
{
    public class GameLobbyHub : Hub
    {
        private readonly IGameService _gameService;

        public GameLobbyHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        /*
            Tries to create a new game and validates the result object from game service
         */
        public async Task CreateGameAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();

            string username = httpContext.GetClaim("username");
            if (username == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", new ResultDto() { Code = 1, Message = "Invalid jwt." });
                return;
            }

            GameConfig gameConfig = new GameConfig(); //Set properties later
            gameConfig.StartHealth = 100;
            gameConfig.StartBulletCount = 2;
            gameConfig.ShootBulletCost = 1;
            gameConfig.ShotgunBulletCost = 3;
            gameConfig.ReloadAmount = 1;
            gameConfig.BulletDamage = 20;
            gameConfig.MinYPosition = 0;
            gameConfig.MaxYPosition = 10;

            gameConfig.MoveDelay = 10;
            gameConfig.ShootDelay = 200;
            gameConfig.ReloadDelay = 500;
            gameConfig.ShotgunDelay = 2000;

            gameConfig.YStartPosition = 100;
            gameConfig.YEndPosition = 600;
            gameConfig.YPositions = 10;
            gameConfig.XPlayerPosition = 100;
            gameConfig.XEnemyPosition = 1200;


            ResultDto result = await _gameService.CreateAsync(username, gameConfig);

            if (result.Code == 1)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", result);
                return;
            }

            //Add client to the game group
            await Groups.AddToGroupAsync(Context.ConnectionId, (string)result.Data);
            
            //Start waiting for player2 on client side
            await Clients.Client(Context.ConnectionId).SendAsync("WaitGameStartAsync", result, result.Data);
        }

        /*
          Tries to join a game and validates the result object from game service depending on the result it sends either to the caller or the group
       */
        public async Task JoinGameAsync(JoinGameSchema schema)
        {
            HttpContext httpContext = Context.GetHttpContext();

            string username = httpContext.GetClaim("username");
            if (username == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", new ResultDto() { Code = 1, Message = "Invalid jwt." });
                return;
            }

            ResultDto result = await _gameService.JoinAsync(username, schema);

            if (result.Code == 1)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", result);
                return;
            }

            GameInitDto gameInit = result.Data as GameInitDto;

            //Add client to game group
            await Groups.AddToGroupAsync(Context.ConnectionId, gameInit.GameId.ToString());

            //Start game on client side
            await Clients.Group(gameInit.GameId.ToString()).SendAsync("StartGameAsync", result, result.Data);
        }
    }
}
