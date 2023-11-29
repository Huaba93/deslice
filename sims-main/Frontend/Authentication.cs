using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace SIMS_Frontend
{
    public class Authentication
    {
        public Authentication()
        {
        }
        public static string GetAuthServiceURL()
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("AuthUrl", out var AuthUrl);
#else
			string AuthUrl = Environment.GetEnvironmentVariable("AuthUrl");
#endif
            if (null == AuthUrl) { Console.WriteLine("FAILURE NO AUTHURL Found"); Environment.Exit(2); }

            return AuthUrl;
        }
        public static ResponseObj PostAuthService(string jsonObject, string PostUri, string? authToken = null)
        {
            string AuthUrl = GetAuthServiceURL();
            HttpResponseMessage response;
            using (HttpClient client = new())
            {
                
                client.BaseAddress = new Uri(AuthUrl);
                StringContent content = new StringContent(jsonObject, System.Text.Encoding.UTF8, "application/json");
                if (null != authToken) { content.Headers.Add("authToken", authToken);}
                using (var task = Task.Run(() => client.PostAsync(PostUri, content)))
                {
                    task.Wait();
                    response = task.Result;
                }
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ResponseObj(HttpStatusCode.Unauthorized);
            }
            string message;
            using (var task = Task.Run(() => response.Content.ReadAsStringAsync()))
            {
                task.Wait();
                message = task.Result;

            }
            ResponseObj resp = new ResponseObj(HttpStatusCode.OK);
            resp.Message = message;

            return resp;
        }
        public static HttpResponseMessage GetAuthService(string GetUri, string? authToken = null)
        {
            string AuthUrl = GetAuthServiceURL();
            HttpResponseMessage message;
            using (HttpClient httpClient = new()){
                using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, new Uri(AuthUrl + GetUri)))
                {

                    requestMessage.Headers.Add("authToken", authToken);
                   
                    using (var task = Task.Run(() => httpClient.SendAsync(requestMessage)))
                    {
                        task.Wait();
                        message = task.Result;
                    }
                }
            }


            return message;
        }

        public static string GetUserName(int UID, string authtoken)
        {

            HttpResponseMessage message = GetAuthService("/User/"+UID.ToString(), authtoken);
            UserRetObj user;
            using (var task = Task.Run(() => message.Content.ReadFromJsonAsync<UserRetObj>()))
            {
                task.Wait();
                user = task.Result;
            }

            return user.Name ?? "User not found"; 
            
        }

        public static ResponseObj GetAuthToken(string Username, string Password)
        {
            string AuthUrl = Authentication.GetAuthServiceURL();
            AuthObj auth = new AuthObj(Username, Password);
            string json = JsonConvert.SerializeObject(auth);
            ResponseObj resp = Authentication.PostAuthService(json, "/auth");
            return resp;

        }

        public static string GetPubKey(bool force = false)
        {
            if (Environment.GetEnvironmentVariable("ecPublicKey") == null || force)
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
            else
            {
                return Environment.GetEnvironmentVariable("ecPublicKey") ?? "Not Found";
            }

        }

        public static  TokenObj ReadJwtToken(string token)
        {
            JsonWebTokenHandler jwtHandler = new();
            string? PemEc = GetPubKey();
            if (null == PemEc) { return new TokenObj(false); }
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

                if (result.IsValid)
                {
                    TokenObj tokenObj = new(true);
                    int.TryParse(result.Claims["userID"].ToString(), out int userId);
                    int.TryParse(result.Claims["roleID"].ToString(), out int roleId);
                    tokenObj.userID = userId;
                    tokenObj.roleID = roleId;
                    return tokenObj;
                }
            }
            catch (Exception ex)
            {
            }
            return new TokenObj(false);
        }

    }
    public class AuthObj
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthObj(string Username, string Password) { this.Username = Username; this.Password = Password; }
    }
    public class TokenObj
    {
        public bool isValid { get; set; } = false;
        public int userID { get; set; } = 0;
        public int roleID { get; set; } = 0;
        public TokenObj(bool tokenValid) { this.isValid = tokenValid; }
        public TokenObj(bool tokenValid, int userID, int roleID) { this.isValid = tokenValid; this.userID = userID; this.roleID = roleID; }
        public TokenObj() { }
    }
    public class ResponseObj
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Response { get; set; }
        public string? Message { get; set; }
        public ResponseObj() { }
        public ResponseObj(HttpStatusCode StatusCode) { this.StatusCode = StatusCode; }
        public ResponseObj(HttpStatusCode StatusCode, string Response) { this.StatusCode = StatusCode; this.Response = Response; }

    }
    public class UserRetObj
    {
        public int UserID { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public byte[]? Salt { get; set; }
        public int RoleID { get; set; }

        public string? Name { get; set; }
    }
}

