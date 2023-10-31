using ShotgunClassLibrary.Models.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsClient.Classes
{
    public class User
    {
        public string Username { get; private set; } = null!;
        public string UserType { get; private set; } = null!;
        public string Jwt { get; private set; } = null!;
        public User(string jwt)
        {
            Jwt = jwt;

            JwtSecurityToken decodedToken = new JwtSecurityToken(jwt);

            Username = decodedToken.Claims.FirstOrDefault(c => c.Type == "username").Value;
            UserType = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
        }
    }
}
