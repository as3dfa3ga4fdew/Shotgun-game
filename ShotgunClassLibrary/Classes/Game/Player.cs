using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Models.Dtos;

namespace ShotgunClassLibrary.Classes.Game
{
    public class Player
    {
        public string Username { get; set; } = null!;
        public int YPosition { get; set; }
        public int Health { get; set; }
        public int BulletCount { get; set; }

        public void Move(int newYPosition)
        {
            YPosition = newYPosition;
        }

        public bool ValidateMove(int newYPosition, int minYPosition, int maxYPosition)
        {
            //Check if out of bounds
            if (newYPosition < minYPosition || newYPosition > maxYPosition)
                return false;

            //Check if the new position is valid
            if (newYPosition != (YPosition + 1) && newYPosition != (YPosition - 1))
                return false;

            return true;
        }

        public void Reload(int newBulletCount)
        {
            BulletCount = newBulletCount;
        }

        public bool ValidateReload(int newBulletCount, int newYPosition)
        {
            if (YPosition != newYPosition)
                return false;

            if (newBulletCount != (BulletCount + 1))
                return false;

            return true;      
        }

        public void Shoot(int bulletCost)
        {
            BulletCount -= bulletCost;
        }

        public void Shotgun(int bulletCost)
        {
            BulletCount -= bulletCost;
        }

        public bool ValidateShootOrShotgun(int bulletCost, int newYPosition)
        {
            if (YPosition != newYPosition)
                return false;

            if (BulletCount < bulletCost)
                return false;

            return true;
        }

        public void Hit(int bulletDamage)
        {
            Health -= bulletDamage;

            return;
        }
    }
}
