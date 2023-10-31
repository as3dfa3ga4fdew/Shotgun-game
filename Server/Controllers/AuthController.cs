using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShotgunClassLibrary.Models.Schemas;
using Server.Services.Interfaces;
using Server.Helpers;
using ShotgunClassLibrary.Dtos;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRsaService _rsaService;
        public AuthController(IAuthService authService, IRsaService rsaService)
        {
            _authService = authService;
            _rsaService = rsaService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginSchema schema)
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            try
            {
                return await _authService.LoginAsync(schema);
            }
            catch (Exception e)
            {
                return StatusCode(500, "");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterSchema schema)
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            try
            {
                return await _authService.RegisterAsync(schema);
            }
            catch(Exception e)
            {
                return StatusCode(500, "");
            }
        }

        [HttpGet("publickey")]
        public async Task<IActionResult> GetPublicKeyAsync()
        {
            return ActionResultHandler.Ok(_rsaService.GetPublicKey());
        }
    }
}
