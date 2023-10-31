using ShotgunClassLibrary.Classes.Game;
using ShotgunClassLibrary.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShotgunClassLibrary.Dtos
{
    public class PlayerActionResponse
    {
        public int SequenceNumber { get; set; }

        public string Username { get; set; } = null!;
        public int YPosition { get; set; }
        public int Health { get; set; }
        public int BulletCount { get; set; }

        public int EnemyHealth { get; set; }

        public PlayerAction Action { get; set; }

        public bool IsValid { get; set; }
    }
}
