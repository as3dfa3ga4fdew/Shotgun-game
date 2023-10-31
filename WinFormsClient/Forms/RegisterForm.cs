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
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsClient.Classes;

namespace WinFormsClient.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly HttpClient _httpClient;
        public User User { get; private set; }
        public RegisterForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["url"]);
        }

        /*
            Validates user input and sends a http request to server to register
         */
        private async void RegisterButton_Click(object sender, EventArgs e)
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

            RegisterSchema schema = new RegisterSchema();
            schema.Username = RsaHandler.Encrypt(rsaPublicKey.RsaPublicKey, UsernameTextBox.Text);
            schema.Password = RsaHandler.Encrypt(rsaPublicKey.RsaPublicKey, PasswordTextBox.Text);

            response = await _httpClient.PostAsJsonAsync("api/auth/register", schema);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    ErrorLabel.Text = "Username already taken!";
                    return;
                }

                ErrorLabel.Text = "Ooops, something went wrong";
                return;
            }

            RegisterDto registerDto = await response.Content.ReadFromJsonAsync<RegisterDto>();
            User = new User(registerDto.Jwt);

            this.Close();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            
        }

        private void BackButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
