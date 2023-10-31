using ShotgunClassLibrary.Classes.Game;
using System.ComponentModel;

namespace ShotgunClassLibrary.Models.Dtos
{
    public class GameItemDto
    {
        [Browsable(false)]
        public Guid Id { get; set; }
        [DisplayName("Host")]
        public string HostUsername { get; set; } = null!;
        [DisplayName("Health")]
        public int StartHealth { get; set; }
        [DisplayName("Bullets")]
        public int StartBullets { get; set; }
    }
}
