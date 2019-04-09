namespace GetReady.Services.Implementations
{
    using System.Linq;
    using System.Security.Claims;
    using GetReady.Services.Contracts;
    using GetReady.Services.Models.JwtModels;
    using GetReady.Services.Utilities;

    public class JwtService : IJwtService
    {
        public JwtUserData ParseData(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = claimsPrincipal.Claims;

            var result = new JwtUserData
            {
                Role = claims.ToArray().SingleOrDefault(x => x.Type == Constants.RoleType).Value,
                UserId = int.Parse(claims.ToArray().SingleOrDefault(x => x.Type == Constants.UserIdType).Value),
            };

            return result;
        }
    }
}
