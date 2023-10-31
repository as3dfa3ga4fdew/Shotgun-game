using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Server.Helpers;
using Server.Helpers.Extensions;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Dtos;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /*
            Gets the users game stats only if the user has premium role
         */
        [HttpGet("stats")]
        [Authorize(Roles = "Premium")]
        public async Task<IActionResult> GetGameStatsAsync()
        {
            //No need to validate since the method has Authorize role
            string accessToken = Request.Headers[HeaderNames.Authorization];

            string username = accessToken.ToString().GetClaim("username");
            if (username == null)
                return ActionResultHandler.BadRequest();


            return  await _userService.GetUserStatsAsync(username);
        }

        [HttpGet("token")]
        public async Task<IActionResult> ValidateToken()
        {
            return NoContent();
        }
    }
}
