using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShotgunClassLibrary.Classes.Game;
using Server.Helpers.Extensions;
using Server.Hubs.Interfaces;
using ShotgunClassLibrary.Models.Dtos;
using Server.Models.Schemas;
using Server.Services;
using Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ShotgunClassLibrary.Schemas;
using ShotgunClassLibrary.Dtos;
using Azure;
using Azure.Core;

namespace Server.Hubs
{
    [Authorize]
    public sealed class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        /*
            Adds a client to its game group if the client is in a game
         */
        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();

            string username = httpContext.GetClaim("username");
            if (username == null)
            {
                await base.OnConnectedAsync();
                return;
            }

            Guid gameId = await _gameService.GetPlayerGameIdAsync(username);
            if(gameId == Guid.Empty)
            {
                await base.OnConnectedAsync();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

            await base.OnConnectedAsync();
        }
        
        /*
            Validates the result from game service and returns the result to the caller client or to the group
         */
        public async Task PlayerActionAsync(PlayerActionRequest request)
        {
            HttpContext httpContext = Context.GetHttpContext();

            string username = httpContext.GetClaim("username");
            if (username == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", new ResultDto() { Code = 1, Message = "Invalid jwt." });
                return;
            }

            ResultDto result = await _gameService.ProcessActionAsync(username, request);

            PlayerActionResponse response = result.Data as PlayerActionResponse;

            if (response == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorAsync", result);
                return;
            }

            if (result.Code == 1)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateGameAsync", response);
                return;
            }


            //Add to group each time
            await Groups.AddToGroupAsync(Context.ConnectionId, request.GameId.ToString());

            Console.WriteLine(JsonConvert.SerializeObject(response));

            if (result.Code == 2)
            {
                await Clients.Group(request.GameId.ToString()).SendAsync("EndGameAsync", response);
                
                return;
            }

            await Clients.Group(request.GameId.ToString()).SendAsync("UpdateGameAsync", response);
        }
    }
}
