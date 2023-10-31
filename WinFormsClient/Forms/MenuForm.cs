using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Server.Models.Schemas;
using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Models.Dtos;
using ShotgunClassLibrary.Models.Schemas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsClient.Classes;

namespace WinFormsClient.Forms
{
    public partial class MenuForm : Form
    {
        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                if (value == null)
                {
                    UsernameLabel.Text = string.Empty;
                    CreateButton.Enabled = false;
                    JoinButton.Enabled = false;
                }
                else
                {
                    UsernameLabel.Text = "Logged in as " + value.Username;
                    CreateButton.Enabled = true;
                    JoinButton.Enabled = true;
                }

                _user = value;
            }
        }

        private HubConnection _lobbyConnection;

        public MenuForm()
        {
            InitializeComponent();

            LogoutButton.Hide();
            JoinButton.Enabled = false;
            CreateButton.Enabled = false;
            WaitingLabel.Hide();

            SetGameListDataGridViewProperties();
        }

        private void SetGameListDataGridViewProperties()
        {
            GameListDataGridView.Columns.Add("HostUsername", "Host");
            GameListDataGridView.Columns.Add("StartHealth", "Health");
            GameListDataGridView.Columns.Add("StartBullets", "Bullets");
            GameListDataGridView.Columns.Add("Id", "ID");

            GameListDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GameListDataGridView.AllowUserToDeleteRows = false;
            GameListDataGridView.AllowUserToAddRows = false;
            GameListDataGridView.ReadOnly = true;
            GameListDataGridView.AllowUserToResizeColumns = false;
            GameListDataGridView.RowHeadersVisible = false;
            GameListDataGridView.AllowUserToResizeRows = false;
            GameListDataGridView.MultiSelect = false;
            GameListDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GameListDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            GameListDataGridView.SelectionChanged += GameListDataGridView_SelectionChanged;
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {

        }

        /*
            Shows/Hides join button depending on if logged in or if the game list grid has any games to join
         */
        private void GameListDataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            if (User == null)
            {
                JoinButton.Enabled = false;
                return;
            }

            if (GameListDataGridView.SelectedRows.Count == 0)
            {
                JoinButton.Enabled = false;
                return;
            }

            int rowIndex = GameListDataGridView.SelectedRows[0].Index;

            if (rowIndex < 0)
            {
                JoinButton.Enabled = false;
                return;
            }

            JoinButton.Enabled = true;
        }

        private async Task StartGameLobbyConnection(string jwt)
        {
            _lobbyConnection = new HubConnectionBuilder()
                .WithUrl(ConfigurationManager.AppSettings["url"] + "game/lobby", HttpTransportType.WebSockets, x => x.Headers.Add("Authorization", "Bearer " + jwt))
                .WithAutomaticReconnect()
                .Build();

            _lobbyConnection.On<ResultDto, List<GameItemDto>>("GameListUpdateAsync", GameListUpdateAsync);
            _lobbyConnection.On<ResultDto, GameInitDto>("StartGameAsync", StartGameAsync);
            _lobbyConnection.On<ResultDto, Guid>("WaitGameStartAsync", WaitGameStartAsync);
            _lobbyConnection.On<ResultDto>("ErrorAsync", ErrorAsync);

            try
            {
                await _lobbyConnection.StartAsync();
            }
            catch (Exception e)
            {

                this.Invoke(new MethodInvoker(delegate ()
                {
                    MessageBox.Show("Could not connect to the server, please try again later.");
                }));
            }
        }
        private async Task StopGameLobbyConnection()
        {
            if (_lobbyConnection.State != HubConnectionState.Connected)
                return;

            await _lobbyConnection.StopAsync();
            await _lobbyConnection.DisposeAsync();
        }

        /*
            While waiting to enter the game form hide and diable all buttons
         */
        private void SwitchHideForGameForm()
        {
            GameListDataGridView.Hide();
            JoinButton.Hide();
            CreateButton.Hide();

            WaitingLabel.Show();
        }

        /*
            When returning from game form show and enable all buttons
         */
        private void SwitchShowForMenuForm()
        {
            GameListDataGridView.Show();
            JoinButton.Show();
            CreateButton.Show();

            WaitingLabel.Hide();
        }


        /*
            Update game list grid with joinable games received from server by Signalr
         */
        public async Task GameListUpdateAsync(ResultDto result, List<GameItemDto> gameItems)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Remove rows from dataGridView if there's no corresponding object in listB
                    for (int i = GameListDataGridView.Rows.Count - 1; i >= 0; i--)
                    {
                        DataGridViewRow row = GameListDataGridView.Rows[i];
                        Guid rowId = (Guid)row.Cells[3].Value; // Assuming the Id is stored in the fourth cell

                        GameItemDto matchingObject = gameItems.FirstOrDefault(item => item.Id == rowId);

                        if (matchingObject == null)
                        {
                            // Use Invoke to remove the row from dataGridView on the UI thread
                            GameListDataGridView.Invoke((Action)(() =>
                            {
                                GameListDataGridView.Rows.RemoveAt(i);
                            }));
                        }
                    }

                    // Add items from listB to dataGridView if they don't exist in dataGridView
                    foreach (GameItemDto item in gameItems)
                    {
                        bool existsInDataGridView = false;

                        // Check if an object with the same Id exists in dataGridView
                        foreach (DataGridViewRow row in GameListDataGridView.Rows)
                        {
                            Guid rowId = (Guid)row.Cells[3].Value; // Assuming the Id is stored in the fourth cell

                            if (item.Id == rowId)
                            {
                                existsInDataGridView = true;
                                break;
                            }
                        }

                        if (!existsInDataGridView)
                        {
                            // Use Invoke to add the object's properties to dataGridView on the UI thread
                            GameListDataGridView.Invoke((Action)(() =>
                            {
                                GameListDataGridView.Rows.Add(item.HostUsername, item.StartHealth, item.StartBullets, item.Id);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // You can also use Invoke to show a message box on the UI thread
                    GameListDataGridView.Invoke((Action)(() =>
                    {
                        MessageBox.Show(ex.Message);
                    }));
                }
            });
        }

        /*
            Start game form when game init dto received from server by Signalr
         */
        public async Task StartGameAsync(ResultDto result, GameInitDto gameInit)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                SwitchHideForGameForm();
            }));

            await Task.Delay(2000);

            await StopGameLobbyConnection();

            this.Hide();

            using (GameForm form = new GameForm(User, gameInit))
            {
                form.ShowDialog();
            }

            this.Show();


            await StartGameLobbyConnection(_user.Jwt);

            this.Invoke(new MethodInvoker(delegate ()
            {
                SwitchShowForMenuForm();
            }));
        }
        /*
            Disables all buttons while waiting for another client to join the game received by Signalr
         */
        public async Task WaitGameStartAsync(ResultDto result, Guid gameId)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                SwitchHideForGameForm();
            }));
        }

        /*
            Shows a messagebox with an error received from server by Signalr
         */
        public async Task ErrorAsync(ResultDto result)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                MessageBox.Show(result.Message);
            }));
        }

        /*
            Shows the login form and waits for it close then it sets the _user property if logged in successfully
         */
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            using (LoginForm form = new LoginForm())
            {
                form.ShowDialog();

                if (form.User == null)
                    return;

                this.User = form.User;

                await StartGameLobbyConnection(_user.Jwt);

                LoginButton.Hide();
                RegisterButton.Hide();
                LogoutButton.Show();
            }
        }
        /*
           Shows the register form and waits for it close then it sets the _user property if registered successfully
        */
        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            using (RegisterForm form = new RegisterForm())
            {
                form.ShowDialog();

                if (form.User == null)
                    return;

                this.User = form.User;

                await StartGameLobbyConnection(_user.Jwt);

                LoginButton.Hide();
                RegisterButton.Hide();
                LogoutButton.Show();
            }

        }
        /*
            Disconnects from lobby and closes the application
         */
        private async void QuitButton_Click(object sender, EventArgs e)
        {
            await StopGameLobbyConnection();

            this.Close();
        }
        /*
            Sets the _user property to null and switches buttons
         */
        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            //set jwt token to null or w/e

            this.User = null;
            LogoutButton.Hide();
            LoginButton.Show();
            RegisterButton.Show();
            if (File.Exists("account.aes"))
                File.Delete("account.aes");

            await StopGameLobbyConnection();
        }

        /*
            Gets the selected game from game list grid and requests to join by sginalr
         */
        private async void JoinButton_Click(object sender, EventArgs e)
        {
            if (GameListDataGridView.SelectedRows.Count < 1)
                return;

            var row = GameListDataGridView.SelectedRows[0];
            var cells = row.Cells;

            GameItemDto gameItemDto = new GameItemDto();

            gameItemDto.Id = (Guid)cells[3].Value;
            gameItemDto.HostUsername = (string)cells[0].Value;
            gameItemDto.StartHealth = (int)cells[1].Value;
            gameItemDto.StartBullets = (int)cells[2].Value;

            JoinGameSchema schema = new JoinGameSchema() { GameId = gameItemDto.Id };

            await _lobbyConnection.SendAsync("JoinGameAsync", schema);
        }
        /*
            Requests to create a game by sginalr
         */
        private async void CreateButton_Click(object sender, EventArgs e)
        {
            if (User == null)
                return;

            await _lobbyConnection.SendAsync("CreateGameAsync");
        }
    }
}
