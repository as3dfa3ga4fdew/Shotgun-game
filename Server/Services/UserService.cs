using Microsoft.AspNetCore.Mvc;
using Server.Helpers;
using Server.Models.Entities;
using Server.Repositories.Interfaces;
using Server.Services.Interfaces;
using ShotgunClassLibrary.Dtos;

namespace Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /*
            Gets the users game stats and returns the mapped data
         */
        public async Task<IActionResult> GetUserStatsAsync(string username)
        {
            UserEntity user = await _userRepository.GetAsync(username);

            return ActionResultHandler.Ok((GameStatsDto)user);
        }
    }
}
