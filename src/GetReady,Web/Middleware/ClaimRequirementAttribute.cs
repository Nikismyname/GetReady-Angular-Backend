namespace GetReady.Web.Middleware
{
    using GetReady.Services.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using System.Security.Claims;

    //https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core/31465227#31465227
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim claim;

        public ClaimRequirementFilter(Claim claim)
        {
            this.claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = false;
            if (claim.Type == Constants.RoleType && claim.Value == "User")
            {
                hasClaim = 
                    context.HttpContext.User.Claims
                        .Any(c => c.Type == Constants.RoleType && c.Value == "User")||
                    context.HttpContext.User.Claims
                        .Any(c => c.Type == Constants.RoleType && c.Value == "Admin");
            }
            else
            {
                hasClaim = context.HttpContext.User.Claims
                    .Any(c => c.Type == claim.Type && c.Value == claim.Value);
            }

            if (!hasClaim)
            {
                context.Result = new ForbidResult("Not Authorized!");
            }
        }
    }
}
