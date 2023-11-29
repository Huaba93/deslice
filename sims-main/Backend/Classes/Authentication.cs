using System;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Azure;

namespace SIMS_Backend
{
    
	public class Authentication
	{
        const int AdminRoleID = 1;
        public static string GetPubKey(bool force = false)
        {
            if(Environment.GetEnvironmentVariable("ecPublicKey") == null || force)
            {
                string ecPubkey = "";
                using (HttpClient client = new HttpClient())
                {
#if DEBUG
                    IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
                    IConfigurationProvider secretProvider = config.Providers.First();
                    secretProvider.TryGet("AuthUrl", out var AuthUrl);
#else
                    string AuthUrl = Environment.GetEnvironmentVariable("AuthUrl") ?? "";
#endif

                    if (AuthUrl == "" || AuthUrl == null) { Console.WriteLine("No Auth Url defined or found!"); Environment.Exit(3); }
                    client.BaseAddress = new Uri(AuthUrl);
                    string PubKey = "";
                    using (var task = Task.Run(() => client.GetStringAsync("Auth/pubkey")))
                    {
                        task.Wait();
                        PubKey = task.Result;
                    }
                    if (PubKey == null || PubKey == "") { Environment.Exit(3); }
                    Environment.SetEnvironmentVariable("ecPublicKey", PubKey);
                    return PubKey;
                }

            }
            else {
                return Environment.GetEnvironmentVariable("ecPublicKey")?? "Not Found"; }
            
        }

        public static AuthObj VerifyJWT(string token)
        {
            JsonWebTokenHandler jwtHandler = new ();
            string? PemEc = GetPubKey();
            if (null == PemEc) { return new AuthObj(false); }
            ECDsa ec = ECDsa.Create();
            ec.ImportFromPem(PemEc);

            try
            {
                jwtHandler.ReadJsonWebToken(token);
                TokenValidationParameters tokenValidparams = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new ECDsaSecurityKey(ec),
                    ValidateAudience = false,
                    ValidateIssuer = false,

                };
                TokenValidationResult result = jwtHandler.ValidateToken(token, tokenValidparams);
                
                if (result.IsValid) {
                    AuthObj authObj = new(true);
                    int.TryParse(result.Claims["userID"].ToString(),out int userId) ;
                    int.TryParse(result.Claims["roleID"].ToString(),out int roleId);
                    authObj.userID = userId;
                    authObj.roleID = roleId;
                    return  authObj; }
            }catch (Exception ex)
            {
                Logservice.writeLog(4, "AuthService", "unkown", "JWT Token check failed: " + ex.Message);
            }
            return new AuthObj(false);
        }

        public static bool isAdmin(AuthObj auth)
        {
            if (auth.roleID == AdminRoleID) { return true; }
            return false;
        }
        public Authentication()
		{

        }
	}
    public class AuthObj
    {
        public bool tokenValid { get; set; } = false;
        public int userID { get; set; } = 0;
        public int roleID { get; set; } = 0;
        public AuthObj(bool tokenValid) { this.tokenValid = tokenValid; }
        public AuthObj(bool tokenValid, int userID, int roleID) { this.tokenValid = tokenValid; this.userID = userID;  this.roleID = roleID; }
        public AuthObj() { }
    }
}

