using Microsoft.AspNetCore.Mvc;
using Server.Helpers;
using ShotgunClassLibrary.Helpers;
using ShotgunClassLibrary.Models.Dtos;
using Server.Models.Entities;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Models.Schemas;

namespace Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IRsaService _rsaService;
        public AuthService(IJwtService jwtService, IUserRepository authRepository, IRsaService rsaService)
        {
            _jwtService = jwtService;
            _userRepository = authRepository;
            _rsaService = rsaService;
        }

        /*
            Checks if credentials are valid and returns a newly created jwt token
         */
        public async Task<IActionResult> LoginAsync(LoginSchema schema)
        {
            //Decrypt values
            schema.Username = _rsaService.Decrypt(schema.Username);
            schema.Password = _rsaService.Decrypt(schema.Password);

            //Validate schema
            if (!ValidateInputs.Username(schema.Username))
                return ActionResultHandler.BadRequest("");

            if (!ValidateInputs.Password(schema.Password))
                return ActionResultHandler.BadRequest("");

            UserEntity user = await _userRepository.GetAsync(schema.Username);
            if(user == null)
                return ActionResultHandler.Unauthorized("");

            if (!BCrypt.Net.BCrypt.Verify(schema.Password, user.Hash))
                return ActionResultHandler.Unauthorized("");

            return ActionResultHandler.Ok(new LoginDto() { Jwt = await _jwtService.GenerateAsync(user) });
        }

        /*
            Validates the schema and creates a new user in the database then creates a jwt token and returns it
         */
        public async Task<IActionResult> RegisterAsync(RegisterSchema schema)
        {

            //Decrypt values
            schema.Username = _rsaService.Decrypt(schema.Username);
            schema.Password = _rsaService.Decrypt(schema.Password);


            //Validate schema
            if (!ValidateInputs.Username(schema.Username))
                return ActionResultHandler.BadRequest("");

            if (!ValidateInputs.Password(schema.Password))
                return ActionResultHandler.BadRequest("");

            //Check if username already used.
            if (await _userRepository.ExistsAsync(schema.Username))
            {
                return ActionResultHandler.Conflict("");
            }

            UserEntity user = new UserEntity();
            user.Username = schema.Username;
            user.UserTypeId = Guid.Parse("b338b4f9-a25e-40d2-9006-d6c09174ca6d"); //Regular
            
            //Hash password
            user.Hash = BCrypt.Net.BCrypt.HashPassword(schema.Password);

            //Add user
            await _userRepository.CreateAsync(user);

            return ActionResultHandler.Ok(new RegisterDto() { Jwt = await _jwtService.GenerateAsync(user) });
        }
    }
}
