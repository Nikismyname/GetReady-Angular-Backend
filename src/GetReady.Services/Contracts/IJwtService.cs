namespace GetReady.Services.Contracts
{
    using GetReady.Services.Models.JwtModels;
    using System.Security.Claims;

    public interface IJwtService
    {
        JwtUserData ParseData(ClaimsPrincipal claims);
    }
}
