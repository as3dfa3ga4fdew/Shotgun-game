using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotgunClassLibrary.Classes.Game
{
    public class Game
    {
        public Guid Id { get; set; }
        public SemaphoreSlim SemaphoreSlim { get; set; } = new SemaphoreSlim(1, 1);
        public GameConfig Config { get; set; }

        public Dictionary<string, Player> Players { get; set; } = null!;

        public bool HasEnded => Players.Values.Where(x => x.Health <= 0).FirstOrDefault() != null;



        public static implicit operator GameInitDto(Game game)
        {
            return new GameInitDto()
            {
                GameId = game.Id,
                GameConfig = game.Config,
                PlayerActionResponses = new List<PlayerActionResponse>()
                {
                    new PlayerActionResponse()
                    {
                        SequenceNumber = 0,
                        Username = game.Players.Values.ElementAt(0).Username,
                        YPosition = game.Players.Values.ElementAt(0).YPosition,
                        Health = game.Players.Values.ElementAt(0).Health,
                        BulletCount = game.Players.Values.ElementAt(0).BulletCount,
                        Action = PlayerAction.Stand,
                        IsValid = true
                    },
                    new PlayerActionResponse()
                    {
                        SequenceNumber = 0,
                        Username = game.Players.Values.ElementAt(1).Username,
                        YPosition = game.Players.Values.ElementAt(1).YPosition,
                        Health = game.Players.Values.ElementAt(1).Health,
                        BulletCount = game.Players.Values.ElementAt(1).BulletCount,
                        Action = PlayerAction.Stand,
                        IsValid = true
                    }
                }
            };
        }
        public static implicit operator GameItemDto(Game game)
        {
            return new GameItemDto()
            {
                Id = game.Id,
                HostUsername = game.Players.Values.First().Username,
                StartHealth = game.Config.StartHealth,
                StartBullets = game.Config.StartBulletCount
            };
        }
    }
}
