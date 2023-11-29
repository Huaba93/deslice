using SIMS_Backend;

namespace Test2;

public class MockAuthenticator : IAuthentication
{
    public readonly string ValidToken = "valid";
    public string InValidToken = "InValidToken";
    public readonly string AdminToken = "admin";
    
    private readonly AuthObj _admin = new AuthObj(true, 1, 1);
    private readonly AuthObj _anyOtherUser = new AuthObj(true, 2, 0);

    public AuthObj VerifyJWT(string token)
    {
        if (token.Equals(ValidToken))
        {
            return _anyOtherUser;
        }

        if (token.Equals(AdminToken))
        {
            return _admin;
        }

        return new AuthObj(false);
    }

    public bool IsAdmin(AuthObj auth)
    {
        return auth.roleID == _admin.roleID;
    }
}