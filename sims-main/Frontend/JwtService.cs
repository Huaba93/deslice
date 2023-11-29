using System;
using System.IdentityModel.Tokens.Jwt;
namespace SIMS_Frontend
{
    public class JwtService
    {
        private static JwtSecurityToken DecryptJwtToken(string jwtToken)
        {
            JwtSecurityTokenHandler jwtHandler = new();
            JwtSecurityToken token =  jwtHandler.ReadJwtToken(jwtToken);
            return token;
        }
        public static int GetUserIdFromToken(string jwtToken)
        {
            JwtSecurityToken token = DecryptJwtToken(jwtToken);
            int.TryParse(token.Claims.First(c => c.Type == "userID").Value,out int uid);
            return uid;
        }
    }
}

