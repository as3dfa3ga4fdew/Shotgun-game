using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using ShotgunClassLibrary.Classes.Game;
using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Models.Dtos;
using ShotgunClassLibrary.Schemas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsClient.Classes;


namespace WinFormsClient.Forms
{
    public partial class GameForm : Form
    {
        private Guid _gameId;
        private User _user;
        private HubConnection _gameConnection;

        private GameConfig _gameConfig;

        private PlayerUi _playerUi;
        private PlayerUi _enemyUi;

        public GameForm(User user, GameInitDto gameInit)
        {
            InitializeComponent();
            _user = user;

            PlayerActionResponse playerAction = gameInit.PlayerActionResponses.Where(x => x.Username == _user.Username).First();
            PlayerActionResponse enemyAction = gameInit.PlayerActionResponses.Where(x => x.Username != _user.Username).First();

            _gameId = gameInit.GameId;
            _gameConfig = gameInit.GameConfig;

            _playerUi = CreatePlayerUi(playerAction, _gameConfig, this, true);
            _enemyUi = CreatePlayerUi(enemyAction, _gameConfig, this, false);
        }

        private async void GameForm_Load(object sender, EventArgs e)
        {
            await StartGameHubConnectionAsync(_user.Jwt);
        }

        private async Task StartGameHubConnectionAsync(string jwt)
        {
            try
            {
                if (_gameConnection != null && _gameConnection.State == HubConnectionState.Connected)
                    return;

                _gameConnection = new HubConnectionBuilder()
                  .WithUrl(ConfigurationManager.AppSettings["url"] + "game", HttpTransportType.WebSockets, x => x.Headers.Add("Authorization", "Bearer " + jwt))
                  .WithAutomaticReconnect()
                  .Build();

                _gameConnection.On<PlayerActionResponse>("UpdateGameAsync", UpdateGameAsync);
                _gameConnection.On<PlayerActionResponse>("EndGameAsync", EndGameAsync);
                _gameConnection.On<ResultDto>("ErrorAsync", ErrorAsync);

                await _gameConnection.StartAsync();
            }
            catch (Exception e)
            {
                //Closes this form if the connection failed
                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show("Couldn't connect to the server!");
                    this.Close();
                }));
            }
        }
        private async Task StopGameHubConnectionAsync()
        {
            await _gameConnection.StopAsync();
            await _gameConnection.DisposeAsync();
        }

        /*
            Creates a playerUi object
         */
        private PlayerUi CreatePlayerUi(PlayerActionResponse playerActionResponse, GameConfig gameConfig, Form form, bool isPlayer)
        {
            int playerXPosition = isPlayer ? gameConfig.XPlayerPosition : gameConfig.XEnemyPosition;

            PlayerUi playerUi = new PlayerUi(playerActionResponse.Username, ImportImages(!isPlayer), ImportSounds());
            playerUi.CreateMap(playerActionResponse.YPosition,
                gameConfig.YStartPosition,
                gameConfig.YEndPosition,
                gameConfig.YPositions,
                playerXPosition,
                form);

            playerUi.CreateHealth(gameConfig.StartHealth,
                playerXPosition,
                form);

            playerUi.CreateBulletCount(gameConfig.StartBulletCount,
                playerXPosition,
                form);


            playerUi.Username = playerActionResponse.Username;
            playerUi.YPosition = playerActionResponse.YPosition;
            playerUi.Health = gameConfig.StartHealth;
            playerUi.BulletCount = gameConfig.StartBulletCount;

            return playerUi;
        }
       
        /*
            Imports images and maps them to a dictionary
         */
        private Dictionary<string, Image> ImportImages(bool rotate)
        {
            string directory = ConfigurationManager.AppSettings["imagesPath"];

            Dictionary<string, Image> images = new Dictionary<string, Image>();

            foreach (var path in Directory.GetFiles(directory))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName.Contains("guy_"))
                    images.Add(Path.GetFileNameWithoutExtension(path), ResizeImage(Image.FromFile(path), 100, 100));

                if (fileName.Contains("icon_"))
                    images.Add(Path.GetFileNameWithoutExtension(path), ResizeImage(Image.FromFile(path), 50, 50));

                if (fileName.Contains("stat_"))
                    images.Add(Path.GetFileNameWithoutExtension(path), ResizeImage(Image.FromFile(path), 20, 20));
            }

            if (rotate)
                foreach (var image in images)
                    image.Value.RotateFlip(RotateFlipType.RotateNoneFlipX);

            return images;
        }

        /*
            Resizes an image
         */
        private Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /*
            Imports sounds and maps them to a dictionary
         */
        private Dictionary<string, SoundPlayer> ImportSounds()
        {
            Dictionary<string, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();
            string directory = ConfigurationManager.AppSettings["soundsPath"];
            foreach (var path in Directory.GetFiles(directory))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                sounds.Add(fileName, new SoundPlayer(path));
            }

            return sounds;
        }

        /*
            Validates user keyboard input and updates the playerUi properties also it sends an action request to the server by Signalr
         */
        protected override async void OnKeyDown(KeyEventArgs e)
        {
            if (_playerUi.InputDisabled)
                return;

            if (_playerUi.SemaphoreSlim.CurrentCount == 0)
                return;

            await _playerUi.SemaphoreSlim.WaitAsync();

            switch (e.KeyCode)
            {
                case Keys.Up:
                    //Validate
                    int newYPosition = _playerUi.YPosition - 1;

                    if (!_playerUi.ValidateMove(newYPosition, 0, _gameConfig.YPositions))
                        break;

                    //Send
                    PlayerActionRequest request = new PlayerActionRequest();
                    request.SequenceNumber = 0;
                    request.YPosition = newYPosition;
                    request.Action = PlayerAction.Move;
                    request.GameId = _gameId;

                    await _gameConnection.SendAsync("PlayerActionAsync", request);

                    await _playerUi.MoveAsync(newYPosition, _gameConfig.MoveDelay);

                    break;
                case Keys.Down:
                    //Validate
                    newYPosition = _playerUi.YPosition + 1;

                    if (!_playerUi.ValidateMove(newYPosition, 0, _gameConfig.YPositions))
                        break;

                    //Send
                    request = new PlayerActionRequest();
                    request.SequenceNumber = 0;
                    request.YPosition = newYPosition;
                    request.Action = PlayerAction.Move;
                    request.GameId = _gameId;

                    await _gameConnection.SendAsync("PlayerActionAsync", request);

                    await _playerUi.MoveAsync(newYPosition, _gameConfig.MoveDelay);

                    break;
                case Keys.X:
                    int newBulletCount = _playerUi.BulletCount + 1;

                    //Send
                    request = new PlayerActionRequest();
                    request.SequenceNumber = 0;
                    request.YPosition = _playerUi.YPosition;
                    request.Action = PlayerAction.Reload;
                    request.GameId = _gameId;

                    await _gameConnection.SendAsync("PlayerActionAsync", request);

                    await _playerUi.ReloadAsync(newBulletCount, _gameConfig.ReloadDelay);
                    break;
                case Keys.Z:
                    if (!_playerUi.ValidateShootOrShotgun(_gameConfig.ShootBulletCost))
                        break;

                    newBulletCount = _playerUi.BulletCount - _gameConfig.ShootBulletCost;

                    //Send
                    request = new PlayerActionRequest();
                    request.SequenceNumber = 0;
                    request.YPosition = _playerUi.YPosition;
                    request.Action = PlayerAction.Shoot;
                    request.GameId = _gameId;

                    await _gameConnection.SendAsync("PlayerActionAsync", request);

                    await _playerUi.ShootAsync(_gameConfig.ShootDelay);
                    _playerUi.UpdateShoot(newBulletCount);

                    if (HasHitWithShoot(_playerUi.YPosition, _enemyUi.YPosition))
                    {
                        int newEnemyHealth = _enemyUi.Health - _gameConfig.BulletDamage;
                        _enemyUi.UpdateHealth(newEnemyHealth);
                    }

                    break;
                case Keys.C:
                    if (!_playerUi.ValidateShootOrShotgun(_gameConfig.ShotgunBulletCost))
                        break;

                    newBulletCount = _playerUi.BulletCount - _gameConfig.ShotgunBulletCost;

                    //Send
                    request = new PlayerActionRequest();
                    request.SequenceNumber = 0;
                    request.YPosition = _playerUi.YPosition;
                    request.Action = PlayerAction.Shotgun;
                    request.GameId = _gameId;

                    await _gameConnection.SendAsync("PlayerActionAsync", request);

                    await _playerUi.ShotgunAsync(_gameConfig.ShotgunDelay);
                    _playerUi.UpdateShoot(newBulletCount);

                    if (HasHitWithShotgun(_playerUi.YPosition, _enemyUi.YPosition))
                    {
                        int newEnemyHealth = _enemyUi.Health - _gameConfig.BulletDamage;
                        _enemyUi.UpdateHealth(newEnemyHealth);
                    }

                    break;
                default:
                    break;
            }

            _playerUi.SemaphoreSlim.Release();
        }

        public bool HasHitWithShoot(int playerYPosition, int enemyYPosition)
        {
            if (playerYPosition != enemyYPosition)
                return false;

            return true;
        }
        public bool HasHitWithShotgun(int playerYPosition, int enemyYPosition)
        {
            if (playerYPosition != enemyYPosition && playerYPosition != (enemyYPosition - 1) && playerYPosition != (enemyYPosition + 1))
                return false;

            return true;
        }


        /*
            Updates playerUi properties by player action response
         */
        private async Task UpdateGameSwitchAsync(PlayerUi playerUi, PlayerUi enemyUi, PlayerActionResponse response)
        {
            await playerUi.SemaphoreSlim.WaitAsync();

            switch (response.Action)
            {
                case PlayerAction.Move:
                    await playerUi.MoveAsync(response.YPosition, _gameConfig.MoveDelay);
                    break;
                case PlayerAction.Reload:
                    await playerUi.ReloadAsync(response.BulletCount, _gameConfig.ReloadDelay);
                    break;
                case PlayerAction.Shoot:
                    await playerUi.ShootAsync(_gameConfig.ShootDelay);
                    playerUi.UpdateShoot(response.BulletCount);
                    enemyUi.UpdateHealth(response.EnemyHealth);
                    break;
                case PlayerAction.Shotgun:
                    await playerUi.ShotgunAsync(_gameConfig.ShotgunDelay);
                    playerUi.UpdateShotgun(response.BulletCount);
                    enemyUi.UpdateHealth(response.EnemyHealth);
                    break;
                case PlayerAction.Stand: //Only to recover
                    playerUi.Stand(response.YPosition, response.BulletCount);
                    break;
                default:
                    break;
            }

            playerUi.SemaphoreSlim.Release();
        }

        /*
            Updates player ui when received from server by Singalr
         */
        public async Task UpdateGameAsync(PlayerActionResponse playerActionResponse)
        {
            this.Invoke(new MethodInvoker(async delegate ()
            {
                if (playerActionResponse.Username == _playerUi.Username) //playerUi update
                {
                    if (playerActionResponse.IsValid)
                        return;

                    //Update playerUi if the response was invalid
                    await UpdateGameSwitchAsync(_playerUi, _enemyUi, playerActionResponse);
                }
                else //enemyUi update
                {
                    await UpdateGameSwitchAsync(_enemyUi, _playerUi, playerActionResponse);
                }
            }));
        }

        /*
            Disables all user input and shows/enables button and label when received from server by Signalr
         */
        public async Task EndGameAsync(PlayerActionResponse playerActionResponse)
        {
            //Update game
            this.Invoke(new MethodInvoker(async delegate ()
            {
                //Update enemy ui if response is from enemy
                if (playerActionResponse.Username != _playerUi.Username)
                {
                    await UpdateGameSwitchAsync(_enemyUi, _playerUi, playerActionResponse);
                }

                //Get winners username
                string winnerUsename = playerActionResponse.Username;

                //Disable input client side
                _playerUi.InputDisabled = true;

                //Create label and show winners username
                Label label = new Label();
                label.Location = new Point(650, 335);
                label.Text = "Winner: " + winnerUsename;
                //font size
                this.Controls.Add(label);
                label.Show();


                //Create button and show quit
                Button button = new Button();
                button.Location = new Point(650, 390);
                button.Click += Quit_Click;
                button.Name = "Quit";
                button.Text = "Quit";
                this.Controls.Add(button);
                button.Show();
            }));
        }

        /*
            Shows a messagebox with an error message received from the server by Signalr
         */
        public async Task ErrorAsync(ResultDto result)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                MessageBox.Show(result.Message);
            }));
        }

         /*
            Disconnects from lobby and closes this form
         */
        private async void Quit_Click(object? sender, EventArgs e)
        {
            await StopGameHubConnectionAsync();

            this.Close();
        }
    }
}
