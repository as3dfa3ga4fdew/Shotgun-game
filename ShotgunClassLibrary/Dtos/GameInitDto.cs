using ShotgunClassLibrary.Classes.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotgunClassLibrary.Dtos
{
    public class GameInitDto
    {
        public Guid GameId { get; set; }
        public GameConfig GameConfig { get; set; } = null!;
        public List<PlayerActionResponse> PlayerActionResponses { get; set; } = null!;
    }
}
