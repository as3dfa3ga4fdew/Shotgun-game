using Microsoft.AspNetCore.Mvc;
using Server.Models.Schemas;
using ShotgunClassLibrary.Models.Schemas;

namespace Server.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<IActionResult> LoginAsync(LoginSchema schema);
        public Task<IActionResult> RegisterAsync(RegisterSchema schema);
    }
}
