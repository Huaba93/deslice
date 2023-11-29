using Microsoft.AspNetCore.Mvc;

namespace SIMS_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class SIMSBaseControlerBase : ControllerBase
{
    protected readonly IAuthentication Auth;
    protected readonly ISIMSContextFactory ContextFactory;

    public SIMSBaseControlerBase(IAuthentication auth, ISIMSContextFactory contextFactory)
    {
        Auth = auth;
        ContextFactory = contextFactory;
    }
}