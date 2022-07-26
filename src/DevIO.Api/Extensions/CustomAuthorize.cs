using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace DevIO.Api.Extensions
{
    public class CustomAuthorization
    {
        #region Public Methods

        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            return context.User.Identity.IsAuthenticated && context.User.Claims.Any(claim => claim.Type.Equals(claimName) && claim.Value.Contains(claimValue));
        }

        #endregion Public Methods
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        #region Public Constructors

        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }

        #endregion Public Constructors
    }

    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        #region Private Fields

        private readonly Claim _claim;

        #endregion Private Fields

        #region Public Constructors

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        #endregion Public Constructors

        #region Public Methods

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }

        #endregion Public Methods
    }
}