using ShotgunClassLibrary.Classes.Game;
using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsClient.Classes
{
    public class PlayerUi
    {
        public string Username { get; set; } = null!;
        public int YPosition { get; set; }
        public int Health { get; set; }
        public int BulletCount { get; set; }

        public SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public bool InputDisabled { get; set; } = false;

        public Dictionary<int, PictureBox> Map { get; set; } = null!;
        public Label HealthLabel { get; set; } = null!;
        public Label BulletCountLabel { get; set; } = null!;
        public Dictionary<string, Image> Images { get; set; } = null!;
        public Dictionary<string, SoundPlayer> Sounds { get; set; } = null!;

        public PlayerUi(string username, Dictionary<string, Image> images, Dictionary<string, SoundPlayer> sounds)
        {
            Username = username;
            Images = images;
            Sounds = sounds;
        }

        /*
            Creates picture boxes and calculates and sets their positions and images and adds it to the game forms control and map
         */
        public void CreateMap(int currentYPosition, int startYPosition, int endYPosition, int totalYPostitions, int playerXPosition, Form form)
        {
            Map = new Dictionary<int, PictureBox>();

            PictureBoxSizeMode pictureBoxSizeMode = PictureBoxSizeMode.CenterImage;

            int yLocationDiff = (endYPosition - startYPosition) / totalYPostitions;

            PictureBox pictureBox;
            for (int i = 0; i < totalYPostitions + 1; i++)
            {
                int yPos = startYPosition + (yLocationDiff * i);

                pictureBox = new PictureBox();
                form.Controls.Add(pictureBox);

                pictureBox.Location = new Point(playerXPosition, yPos);

                pictureBox.Size = new Size(100, 100);
                pictureBox.SizeMode = pictureBoxSizeMode;
                pictureBox.Hide();
                pictureBox.Image = Images["guy_move"];
                Map[i] = pictureBox;
            }

            Map[currentYPosition].Show();
        }

        /*
            Creates a label and pricturebox and adds it to the game forms control
         */
        public void CreateHealth(int startHealth, int playerXPosition, Form form)
        {
            PictureBoxSizeMode pictureBoxSizeMode = PictureBoxSizeMode.CenterImage;

            PictureBox pictureBox = new PictureBox();
            form.Controls.Add(pictureBox);
            pictureBox.Location = new Point(playerXPosition, 10);

            pictureBox.SizeMode = pictureBoxSizeMode;
            pictureBox.Image = Images["stat_heart"];

            HealthLabel = new Label();
            form.Controls.Add(HealthLabel);
            HealthLabel.Text = startHealth.ToString();
            HealthLabel.AutoSize = true;
            HealthLabel.Location = new Point(playerXPosition + 100, 30);
        }

        /*
            Creates a label and pricturebox and adds it to the game forms control
         */
        public void CreateBulletCount(int startBulletCount, int playerXPosition, Form form)
        {
            PictureBoxSizeMode pictureBoxSizeMode = PictureBoxSizeMode.CenterImage;

            PictureBox pictureBox = new PictureBox();
            form.Controls.Add(pictureBox);
            pictureBox.Location = new Point(playerXPosition, 50);

            pictureBox.SizeMode = pictureBoxSizeMode;
            pictureBox.Image = Images["stat_bullet"];

            BulletCountLabel = new Label();
            form.Controls.Add(BulletCountLabel);
            BulletCountLabel.Text = startBulletCount.ToString();
            BulletCountLabel.AutoSize = true;
            BulletCountLabel.Location = new Point(playerXPosition + 100, 70);
        }


        /*
            Validate/Move: Validates inputs and updates playerui properties
            
            Async methods updates the picture boxes by hiding/showing y positions and plays a sound.
         */
        public async Task MoveAsync(int newYPosition, int delay)
        {
            Map[YPosition].Hide();
            Map[YPosition].Image = null;

            YPosition = newYPosition;

            Sounds["sound_move"].Play();

            await Task.Delay(delay);

            Map[newYPosition].Image = Images["guy_move"];
            Map[newYPosition].Show();

            return;
        }
        public bool ValidateMove(int newYPosition, int minYPosition, int maxYPosition)
        {
            if (newYPosition < minYPosition || newYPosition > maxYPosition)
                return false;

            return true;
        }

        public async Task ReloadAsync(int newBulletCount, int delay)
        {
            Map[YPosition].Image = Images["guy_reload"];

            Sounds["sound_reload"].Play();

            await Task.Delay(delay);

            Map[YPosition].Image = Images["guy_stand"];

            BulletCount = newBulletCount;

            BulletCountLabel.Text = BulletCount.ToString();

            return;
        }

        public async Task ShootAsync(int delay)
        {
            Map[YPosition].Image = Images["guy_shoot"];

            Sounds["sound_shoot"].Play();

            await Task.Delay(delay);

            Map[YPosition].Image = Images["guy_stand"];

            return;
        }
        public void UpdateShoot(int newBulletCount)
        {
            BulletCount = newBulletCount;
            BulletCountLabel.Text = newBulletCount.ToString();
        }

        public async Task ShotgunAsync(int delay)
        {
            Map[YPosition].Image = Images["guy_shotgun"];

            Sounds["sound_shotgun"].Play();

            await Task.Delay(delay);

            Map[YPosition].Image = Images["guy_stand"];

            return;
        }
        public void UpdateShotgun(int newBulletCount)
        {
            BulletCount = newBulletCount;
            BulletCountLabel.Text = newBulletCount.ToString();
        }

        public bool ValidateShootOrShotgun(int bulletCost)
        {
            if (BulletCount < bulletCost)
            {
                Sounds["sound_noboooollets"].Play();
                return false;
            }

            return true;
        }

        public void UpdateHealth(int newHealth)
        {
            if (newHealth < 0)
                return;

            Health = newHealth;
            HealthLabel.Text = Health.ToString();

            if (Health == 0)
                Sounds["sound_die"].Play();
        }

        public void Stand(int newYPosition, int bulletCount)
        {
            Map[YPosition].Hide();
            Map[YPosition].Image = null;

            Map[newYPosition].Image = Images["guy_stand"];
            Map[newYPosition].Show();

            YPosition = newYPosition;
            BulletCount = bulletCount;
        }
    }
}
