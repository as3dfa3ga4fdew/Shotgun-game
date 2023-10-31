using System.IdentityModel.Tokens.Jwt;

namespace Server.Helpers.Extensions
{
    public static class StringExtensions
    {
        /*
            Returns a claim from a jwt token by type
         */
        public static string GetClaim(this string jwt, string type)
        {
            //Parse token
            if (jwt == null) return null;

            string[] jwtItems = jwt.Split("Bearer ");

            if (jwtItems.Length != 2) return null;

            //Decode jwt token
            JwtSecurityToken decodedToken = new JwtSecurityToken(jwtItems[1]);

            return decodedToken.Claims.FirstOrDefault(c => c.Type == type).Value;
        }
    }
}
