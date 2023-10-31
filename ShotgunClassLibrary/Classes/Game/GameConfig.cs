namespace ShotgunClassLibrary.Classes.Game
{
    public class GameConfig
    {
        public int StartHealth { get; set; }
        public int StartBulletCount { get; set; }

        public int ShootBulletCost { get; set; }
        public int ShotgunBulletCost { get; set; }

        public int ReloadAmount { get; set; }

        public int BulletDamage { get; set; }

        public int MinYPosition { get; set; }
        public int MaxYPosition { get; set; }

        //Ui
        public int YStartPosition { get; set; }
        public int YEndPosition { get; set; }
        public int YPositions { get; set; }
        public int XPlayerPosition { get; set; }
        public int XEnemyPosition { get; set; }

        public int MoveDelay { get; set; }
        public int ShootDelay { get; set; }
        public int ReloadDelay { get; set; }
        public int ShotgunDelay { get; set; }
    }
}
