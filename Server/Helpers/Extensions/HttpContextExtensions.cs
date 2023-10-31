using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace Server.Helpers.Extensions
{
    public static class HttpContextExtensions
    {
        /*
            Returns a claim from a jwt token by type
         */
        public static string GetClaim(this HttpContext context, string type)
        {
            //Parse token
            string jwt = context.Request.Headers.Authorization.ToString();

            if (jwt == null) return null;

            string[]jwtItems = jwt.Split("Bearer ");

            if(jwtItems.Length != 2) return null;

            //Decode jwt token
            JwtSecurityToken decodedToken = new JwtSecurityToken(jwtItems[1]);

            return decodedToken.Claims.FirstOrDefault(c => c.Type == type).Value;
        }
    }
}
