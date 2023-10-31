using ShotgunClassLibrary.Classes.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotgunClassLibrary.Schemas
{
    public class PlayerActionRequest
    {
        public int SequenceNumber { get; set; }

        public int YPosition { get; set; }
        public PlayerAction Action { get; set; }

        public Guid GameId { get; set; }
    }
}
