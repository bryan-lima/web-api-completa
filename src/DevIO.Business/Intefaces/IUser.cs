using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevIO.Business.Intefaces
{
    public interface IUser
    {
        #region Public Properties

        string Name { get; }

        #endregion Public Properties

        #region Public Methods

        Guid GetUserId();
        string GetUserEmail();
        bool IsAuthenticated();
        bool IsInRole(string role);
        IEnumerable<Claim> GetClaimsIdentity();

        #endregion Public Methods
    }
}