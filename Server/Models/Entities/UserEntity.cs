using ShotgunClassLibrary.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.Entities
{
    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Hash { get; set; } = null!;

        public Guid UserTypeId { get; set; }
        public UserTypeEntity UserType { get; set; } = null!;

        public int Wins { get; set; }
        public int Losses { get; set; } 

        public static implicit operator GameStatsDto(UserEntity user)
        {
            return new GameStatsDto()
            {
                Wins = user.Wins,
                Losses = user.Losses
            };
        }
    }
}
