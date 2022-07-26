using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevIO.Api.Extensions
{
    public class AspNetUser : IUser
    {
        #region Private Fields

        private readonly IHttpContextAccessor _accessor;

        #endregion Private Fields

        #region Public Constructors

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Name => _accessor.HttpContext.User.Identity.Name;

        #endregion Public Properties

        #region Public Methods

        public Guid GetUserId()
        {
            return IsAuthenticated() ? Guid.Parse(_accessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }

        public string GetUserEmail()
        {
            return IsAuthenticated() ? _accessor.HttpContext.User.GetUserEmail() : string.Empty;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return _accessor.HttpContext.User.IsInRole(role);
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        #endregion Public Methods
    }

    public static class ClaimsPrincipalExtensions
    {
        #region Public Methods

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal is null)
                throw new ArgumentException(nameof(principal));

            Claim _claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return _claim?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal is null)
                throw new ArgumentException(nameof(principal));

            Claim _claim = principal.FindFirst(ClaimTypes.Email);
            return _claim?.Value;
        }

        #endregion Public Methods
    }
}