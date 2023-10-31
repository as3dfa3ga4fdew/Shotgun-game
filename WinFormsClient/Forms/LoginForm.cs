using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShotgunClassLibrary.Dtos;
using ShotgunClassLibrary.Helpers;
using ShotgunClassLibrary.Models.Dtos;
using ShotgunClassLibrary.Models.Schemas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsClient.Classes;

namespace WinFormsClient.Forms
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _httpClient;
        public User User { get; private set; }
        private readonly string _accountFilePath = "account.aes";
        public LoginForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["url"]);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
            Validates user input and sends a http request to server for authentication
         */
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs.Username(UsernameTextBox.Text))
            {
                ErrorLabel.Text = "Username is invalid.";
                return;
            }

            if (!ValidateInputs.Password(PasswordTextBox.Text))
            {
                ErrorLabel.Text = "Password is invalid";
                return;
            }

            /*Get rsa public key*/
            HttpResponseMessage response = await _httpClient.GetAsync("api/auth/publickey");
            RsaPublicKeyDto rsaPublicKey = await response.Content.ReadFromJsonAsync<RsaPublicKeyDto>();
            MessageBox.Show(rsaPublicKey.RsaPublicKey);
            LoginSchema schema = new LoginSchema();
            schema.Username = RsaHandler.Encrypt(rsaPublicKey.RsaPublicKey, UsernameTextBox.Text);
            schema.Password = RsaHandler.Encrypt(rsaPublicKey.RsaPublicKey, PasswordTextBox.Text);

            response = await _httpClient.PostAsJsonAsync("api/auth/login", schema);
            if (!response.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Invalid username or password";
                return;
            }

            LoginDto loginDto = await response.Content.ReadFromJsonAsync<LoginDto>();

            User = new User(loginDto.Jwt);

            if (RememberMeCheckBox.Checked)
            {
                await File.WriteAllTextAsync("account.aes", AesHandler.EncryptString(null, schema.Username + "\t" + schema.Password));
            }

            this.Close();
        }

        /*
            Checks if the user enabled remember me and tries to login using those credentials
         */
        private async void LoginForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists(_accountFilePath))
                return;

            ErrorLabel.Text = "Logging in...";
            LoginButton.Enabled = false;
            BackButton.Enabled = false;

            string decDetails = AesHandler.DecryptString(null, await File.ReadAllTextAsync(_accountFilePath));
            string[] decDetailParts = decDetails.Split("\t");

            LoginSchema schema = new LoginSchema();
            schema.Username = decDetailParts[0];
            schema.Password = decDetailParts[1];

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/login", schema);
            if (!response.IsSuccessStatusCode)
            {
                ErrorLabel.Text = "Invalid username or password, please login again.";
                LoginButton.Enabled = true;
                BackButton.Enabled = true;
                return;
            }

            LoginDto loginDto = await response.Content.ReadFromJsonAsync<LoginDto>();

            User = new User(loginDto.Jwt);

            this.Close();
        }
    }
}
