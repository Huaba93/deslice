using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text.Json;


namespace SIMS_Authentication
{
    public class Authentication
	{
        const int keySize = 128;
        const int iterations = 350000;

        public static string? PasswordHash(string password, out byte[] salt)
		{
            salt = RandomNumberGenerator.GetBytes(keySize);
            return GeneratePasswordHash(password, salt);
        }
		private static string GeneratePasswordHash(string password, byte[] salt)
		{
#if DEBUG
            byte[] pepper = Convert.FromHexString("c42dac07ba0d50ad7baab5e04c1e56baa515288444aaaacc29e04bb19063dfcfaf57d505a455dca0a4ed160b34436df3939749bfa026a35f69caadd2a784fb5f5faae00eb26b898262887804019b84956a54089e648bf23c2559e6296745ab83afbce7e8bf1272ff4542f6d3aa62e49ea00ec80b905bcc9ff994e0c8dbb71071");

#else

            byte[] pepper = Convert.FromHexString(Environment.GetEnvironmentVariable("AuthPepper"));

#endif
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            
            using (var hmac = new HMACSHA512(pepper))
            {
                byte[] initialHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(Rfc2898DeriveBytes.Pbkdf2(initialHash, salt, iterations, hashAlgorithm, keySize));

            }
        }
		public static bool VerifyPassword(string password, string hash, byte[] salt)
		{
			byte[] hashToCompare = Convert.FromHexString(GeneratePasswordHash(password, salt));
			return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
		}

        public static bool VerifyJWT(string token)
        {
            JsonWebTokenHandler jwtHandler = new JsonWebTokenHandler();
            string? PemEc = System.Environment.GetEnvironmentVariable("ecPublicKey");
            if (null == PemEc) { return false; }
            ECDsa ec = ECDsa.Create();
            ec.ImportFromPem(PemEc);


            jwtHandler.ReadJsonWebToken(token);
            TokenValidationParameters tokenValidparams = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new ECDsaSecurityKey(ec),
                ValidateAudience = false,
                ValidateIssuer  = false,
                
            };
            TokenValidationResult result = jwtHandler.ValidateToken(token, tokenValidparams);
            if (result.IsValid) { return true; }
            return false;
        }
        public static string GenerateAuthToken(User user)
        {
            JwtHeader header = new();
           

            JwtBody body = new();
            body.username = user.Username;
            body.roleID = user.RoleID;
            body.userID = user.UserID;
            

            string jwtToken = (header.getBase64() + "." + body.getBase64());
            ECDsa eCDsa = ECDsa.Create();

            if (System.Environment.GetEnvironmentVariable("ecPrivKey") != null)
            {
                byte[] privateKey = Convert.FromHexString(System.Environment.GetEnvironmentVariable("ecPrivKey")??"");
                eCDsa.ImportECPrivateKey(source: privateKey, bytesRead: out _);
            }
            else
            {
                eCDsa = GenerateECKey();

            }
            byte[] signJwtToken = eCDsa.SignData(System.Text.Encoding.UTF8.GetBytes(jwtToken),HashAlgorithmName.SHA256);

            string sign = Base64UrlEncoder.Encode(signJwtToken);
            jwtToken = jwtToken + "." + sign;
            return jwtToken;
        }
     
        public static ECDsa GenerateECKey()
        {

            ECDsa eCDsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            System.Environment.SetEnvironmentVariable("ecPrivKey", Convert.ToHexString(eCDsa.ExportECPrivateKey()));
            System.Environment.SetEnvironmentVariable("ecPublicKey", eCDsa.ExportSubjectPublicKeyInfoPem());
            return eCDsa;

        }
        public static long CurrentUnixTimestamp()
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            return dto.ToUnixTimeSeconds();
        }
    }



    public class JwtHeader
    {
        public string alg { get; set; } = "ES256";
        public string typ { get; set; } = "JWT";
        public string getBase64()
        {
           return Base64UrlEncoder.Encode(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this)));
        }
    }

    public class JwtBody
    {
        public long iat { get; } = Authentication.CurrentUnixTimestamp();
        public long exp { get; set; } = Authentication.CurrentUnixTimestamp() + (15 * 60);
        public string username { get; set; } = "";
        public int userID { get; set; } = 0;
        public int roleID { get; set; } = 0;
        public string getBase64()
        {
            return Base64UrlEncoder.Encode(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this)));
        }
    }
}

