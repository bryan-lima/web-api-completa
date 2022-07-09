using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DevIO.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        #region Private Fields

        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        #endregion Private Fields

        #region Public Constructors

        public AuthController(ILogger<AuthController> logger,
                              INotificador notificador,
                              IOptions<AppSettings> appSettings,
                              SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IUser user) : base(notificador, user)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region POST

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            IdentityUser _user = new()
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            IdentityResult _result = await _userManager.CreateAsync(_user, registerUser.Password);

            if (_result.Succeeded)
            {
                await _signInManager.SignInAsync(_user, false);
                return CustomResponse(await GerarJwt(_user.Email));
            }

            foreach (IdentityError error in _result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            SignInResult _result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (_result.Succeeded)
            {
                _logger.LogInformation($"Usuário {loginUser.Email} logado com sucesso");
                return CustomResponse(await GerarJwt(loginUser.Email));
            }

            if (_result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuário ou senha incorretos");
            return CustomResponse(loginUser);
        }

        #endregion POST

        #region Private Methods

        private async Task<LoginResponseViewModel> GerarJwt(string email)
        {
            IdentityUser _user = await _userManager.FindByEmailAsync(email);
            IList<Claim> _claims = await _userManager.GetClaimsAsync(_user);
            IList<string> _userRoles = await _userManager.GetRolesAsync(_user);

            _claims.Add(new Claim(JwtRegisteredClaimNames.Sub, _user.Id));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Email, _user.Email));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            _claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (string userRole in _userRoles)
                _claims.Add(new Claim("role", userRole));

            ClaimsIdentity _identityClaims = new();
            _identityClaims.AddClaims(_claims);

            JwtSecurityTokenHandler _tokenHandler = new();
            byte[] _key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityToken _token = _tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = _identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            });

            string _encodedToken = _tokenHandler.WriteToken(_token);

            LoginResponseViewModel _response = new()
            {
                AccessToken = _encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Id = _user.Id,
                    Email = _user.Email,
                    Claims = _claims.Select(claim => new ClaimViewModel { Type = claim.Type, Value = claim.Value })
                },
            };

            return _response;
        }

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        #endregion Private Methods
    }
}
