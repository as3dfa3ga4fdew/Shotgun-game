using System.ComponentModel.DataAnnotations;

namespace Server.Models.Entities
{
    public class UserTypeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<UserEntity> Users { get; set; }
    }
}
