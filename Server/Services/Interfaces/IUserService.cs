using Microsoft.AspNetCore.Mvc;

namespace Server.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IActionResult> GetUserStatsAsync(string username);
    }
}
