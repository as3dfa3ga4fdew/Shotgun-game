using ShotgunClassLibrary.Classes.Game;

namespace Server.Models.Schemas
{
    public class JoinGameSchema
    {
        public Guid GameId { get; set; }

        public static implicit operator Guid(JoinGameSchema schema)
        {
            return schema.GameId;
        }
    }
}
