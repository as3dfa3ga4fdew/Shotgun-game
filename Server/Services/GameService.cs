using ShotgunClassLibrary.Classes.Game;
using Server.Services.Interfaces;
using Newtonsoft.Json;
using ShotgunClassLibrary.Models.Dtos;
using Server.Models.Entities;
using Server.Models.Schemas;
using System.Collections.Concurrent;
using ShotgunClassLibrary.Schemas;
using ShotgunClassLibrary.Dtos;
using Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Server.Repositories;

namespace Server.Services
{
    public class GameService : IGameService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private ConcurrentDictionary<Guid, Game> _games;

        public GameService(IServiceScopeFactory scopeFactory)
        {
            _games = new ConcurrentDictionary<Guid, Game>();
            _scopeFactory = scopeFactory;
        }

        /*
            Returns the game object id by player username.
         */
        public async Task<Guid> GetPlayerGameIdAsync(string username)
        {
            Game game = _games.Values.Where(x => x.Players.Keys.Any(y => y == username)).FirstOrDefault();

            return game == null ? Guid.Empty : game.Id;
        }

        /*
            Creates a game object and adds the caller user to it.
         */
        public async Task<ResultDto> CreateAsync(string username, GameConfig gameConfig)
        {
            //Check if user already in a game
            if (_games.Values.Where(x => x.Players.Keys.Any(y => y == username)).FirstOrDefault() != null)
                return new ResultDto() { Code = 1, Message = "You are already in a game!" };

            //Create player object
            Player player = new Player();
            player.Username = username;
            player.YPosition = Random.Shared.Next(gameConfig.MinYPosition, gameConfig.MaxYPosition);
            player.Health = gameConfig.StartHealth;
            player.BulletCount = gameConfig.StartBulletCount;

            //Create new game object
            Game game = new Game();
            game.Id = Guid.NewGuid();
            game.Config = gameConfig;
            game.Players = new Dictionary<string, Player>()
            {
                { username, player }
            };
            
            //Add game to list
            if (!_games.TryAdd(game.Id, game))
                return new ResultDto() { Code = 1, Message = "Unable to create the game, please try again." };

            return new ResultDto() { Code = 0, Message = "", Data = game.Id.ToString() };
        }

        /*
            Updates a game object with a new player
         */
        public async Task<ResultDto> JoinAsync(string username, Guid gameId)
        {
            //Check if user already in a game
            if (_games.Values.Where(x => x.Players.Keys.Any(y => y == username)).FirstOrDefault() != null)
                return new ResultDto() { Code = 1, Message = "You are already in a game!" };

            if (!_games.TryGetValue(gameId, out Game game))
                return new ResultDto() { Code = 1, Message = "Game does not exist!" };

            //Lock access to this game object while updating it
            await game.SemaphoreSlim.WaitAsync();

            if (game.Players.Count == 2)
                return new ResultDto() { Code = 1, Message = "Game is full!" };

            Player player = new Player();
            player.Username = username;
            player.YPosition = Random.Shared.Next(game.Config.MinYPosition, game.Config.MaxYPosition);
            player.Health = game.Config.StartHealth;
            player.BulletCount = game.Config.StartBulletCount;

            game.Players.Add(username, player);

            //Release the lock
            game.SemaphoreSlim.Release();

            return new ResultDto() { Code = 0, Message = "", Data = (GameInitDto)game };
        }

        /*
            Gets all joinable games
         */
        public async Task<ResultDto> GetQueueAsync()
        {
            List<GameItemDto> joinableGames = _games.Values.Where(x => x.Players.Count == 1).Select(x => (GameItemDto)x).ToList();

            return new ResultDto() { Code = 0, Message = "", Data = joinableGames };
        }

        /*
            Validates and updates a game object depedning on the player request
         */
        public async Task<ResultDto> ProcessActionAsync(string username, PlayerActionRequest playerActionRequest)
        {
            ResultDto result = new ResultDto();

            //Check if game exists
            if (!_games.TryGetValue(playerActionRequest.GameId, out Game game))
                return new ResultDto() { Code = 1, Message = "Game does not exist!" };

            //Lock this game object
            await game.SemaphoreSlim.WaitAsync();

            //Check if game has started
            if (game.Players.Count != 2)
                return new ResultDto() { Code = 1, Message = "Game has not started!" };

            //Check if player exists in this game
            if (!game.Players.TryGetValue(username, out Player player))
                return new ResultDto() { Code = 1, Message = "You are not in this game!" };

            Player enemy = game.Players.Values.Where(x => x.Username != username).First();

            PlayerActionResponse response = new PlayerActionResponse();
            response.SequenceNumber = playerActionRequest.SequenceNumber;
            response.Username = username;

            try
            {
                //Validates playerActionRequest and updates game object
                switch (playerActionRequest.Action)
                {
                    case PlayerAction.Move:
                        //Validate
                        if (!player.ValidateMove(playerActionRequest.YPosition, game.Config.MinYPosition, game.Config.MaxYPosition))
                        {
                            response.YPosition = player.YPosition;
                            response.Health = player.Health;
                            response.BulletCount = player.BulletCount;
                            response.Action = PlayerAction.Stand;
                            response.IsValid = false;

                            result = new ResultDto() { Code = 1, Message = "Not valid action!", Data = response };
                            break;
                        }

                        //Update
                        player.Move(playerActionRequest.YPosition);

                        //Send
                        response.YPosition = player.YPosition;
                        response.Health = player.Health;
                        response.BulletCount = player.BulletCount;
                        response.Action = playerActionRequest.Action;
                        response.IsValid = true;

                        result = new ResultDto() { Code = 0, Message = "", Data = response };
                        break;
                    case PlayerAction.Reload:
                        int newBulletCount = player.BulletCount + game.Config.ReloadAmount;
                        //Validate
                        if (!player.ValidateReload(newBulletCount, playerActionRequest.YPosition))
                        {
                            response.YPosition = player.YPosition;
                            response.Health = player.Health;
                            response.BulletCount = player.BulletCount;
                            response.Action = PlayerAction.Stand;
                            response.IsValid = false;

                            result = new ResultDto() { Code = 1, Message = "Not valid action!", Data = response };
                            break;
                        }

                        //Update
                        player.Reload(newBulletCount);

                        //Send
                        response.YPosition = player.YPosition;
                        response.Health = player.Health;
                        response.BulletCount = player.BulletCount;
                        response.Action = playerActionRequest.Action;
                        response.IsValid = true;

                        result = new ResultDto() { Code = 0, Message = "", Data = response };
                        break;
                    case PlayerAction.Shoot:
                        //Validate
                        if (!player.ValidateShootOrShotgun(game.Config.ShootBulletCost, playerActionRequest.YPosition))
                        {
                            response.YPosition = player.YPosition;
                            response.Health = player.Health;
                            response.BulletCount = player.BulletCount;
                            response.Action = PlayerAction.Stand;
                            response.EnemyHealth = enemy.Health;
                            response.IsValid = false;

                            result = new ResultDto() { Code = 1, Message = "Invalid action!", Data = response };
                            break;
                        }

                        //Update
                        player.Shoot(game.Config.ShootBulletCost);
                        if (HasHitWithShoot(player.YPosition, enemy.YPosition))
                            enemy.Hit(game.Config.BulletDamage);

                        //Send
                        response.YPosition = player.YPosition;
                        response.Health = player.Health;
                        response.BulletCount = player.BulletCount;
                        response.Action = playerActionRequest.Action;
                        response.EnemyHealth = enemy.Health;
                        response.IsValid = true;

                        result = new ResultDto() { Code = 0, Message = "", Data = response };
                        break;
                    case PlayerAction.Shotgun:
                        //Validate
                        if (!player.ValidateShootOrShotgun(game.Config.ShotgunBulletCost, playerActionRequest.YPosition))
                        {
                            response.YPosition = player.YPosition;
                            response.Health = player.Health;
                            response.BulletCount = player.BulletCount;
                            response.Action = PlayerAction.Stand;
                            response.EnemyHealth = enemy.Health;
                            response.IsValid = false;

                            result = new ResultDto() { Code = 1, Message = "Invalid action!", Data = response };
                            break;
                        }

                        //Update
                        player.Shotgun(game.Config.ShotgunBulletCost);
                        if (HasHitWithShotgun(player.YPosition, enemy.YPosition))
                            enemy.Hit(game.Config.BulletDamage);

                        //Send
                        response.YPosition = player.YPosition;
                        response.Health = player.Health;
                        response.BulletCount = player.BulletCount;
                        response.Action = playerActionRequest.Action;
                        response.EnemyHealth = enemy.Health;
                        response.IsValid = true;

                        result = new ResultDto() { Code = 0, Message = "", Data = response };
                        break;
                    default:
                        result = new ResultDto() { Code = 1, Message = "Not a valid action type!" };
                        break;
                }

            }
            catch(Exception e)
            {
                //log
            }
            finally
            {
                //Release game object from lock
                game.SemaphoreSlim.Release();
            }

            if(game.HasEnded)
            {
                result.Code = 2;

                //Removes game from _games so that the players can create/join new games
                if (!_games.TryRemove(playerActionRequest.GameId, out _))
                {
                    result.Code = 1;
                    result.Message = "Something went wrong.";
                }

                string winnerUsername = username;
                string loserUsername = game.Players.Keys.Where(x => x != winnerUsername).First();

                await UpdateGameStatsAsync(winnerUsername, true);
                await UpdateGameStatsAsync(loserUsername, false);
            }

            return result;
        }

        /*
            Updates a users game stats in the database
         */
        private async Task UpdateGameStatsAsync(string username, bool winner)
        {
            //Get scope and dispose it since this service is singleton
            using (var scope = _scopeFactory.CreateScope())
            {
                IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                UserEntity user = await userRepository.GetAsync(username);
                if (winner)
                    user.Wins++;
                else
                    user.Losses++;

                await userRepository.UpdateAsync(user);
            }
        }

        private bool HasHitWithShoot(int playerYPosition, int enemyYPosition)
        {
            if (playerYPosition != enemyYPosition)
                return false;

            return true;
        }
        private bool HasHitWithShotgun(int playerYPosition, int enemyYPosition)
        {
            if (playerYPosition != enemyYPosition && playerYPosition != (enemyYPosition - 1) && playerYPosition != (enemyYPosition + 1))
                return false;

            return true;
        }
    }
}
