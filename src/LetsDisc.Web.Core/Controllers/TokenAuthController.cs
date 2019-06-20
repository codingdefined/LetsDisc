using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using LetsDisc.Authentication.External;
using LetsDisc.Authentication.JwtBearer;
using LetsDisc.Authorization;
using LetsDisc.Authorization.Users;
using LetsDisc.Models.TokenAuth;
using LetsDisc.MultiTenancy;
using LetsDisc.Identity;
using Microsoft.AspNetCore.Cors;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Authentication;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LetsDisc.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : LetsDiscControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly SignInManager _signInManager;
        private readonly UserManager _userManager;
        private readonly IAbpSession _session;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            SignInManager signInManager,
            UserManager userManager,
            IAbpSession session)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _session = session;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id
            };
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }

        [EnableCors("localhost")]
        [HttpGet]
        public IActionResult SignInWithExternalProvider(string provider)
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties(provider, Url.Action(nameof(ExternalLoginCallback), new { p = provider }));
            return Challenge(authenticationProperties, provider);
        }

        [HttpGet]
        public bool IsUserAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallback(string p, string returnUrl = null, string remoteError = null)
        {

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }
            var externalUser = result.Principal;
            if (externalUser == null)
            {
                throw new Exception("External authentication error");
            }

            var claims = externalUser.Claims.ToList();
            var userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            var userIdEmail = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if(userIdEmail == null)
            {
                throw new Exception("Invalid Email");
            }
            var userInDB = await _userManager.FindByEmailAsync(userIdEmail.Value);

            if(userInDB == null)
            {
                await RegisterForExternalLogin(claims);
            }


            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return returnToHomeURL();

            /*var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);

            if (!result.Succeeded) //user does not exist yet
            {
                
            }*/

            /*if(externalLoginInfo != null)
            {
                var tenancyName = GetTenancyNameOrNull();
                var loginResult = await _logInManager.LoginAsync(externalLoginInfo, tenancyName);
                switch (loginResult.Result)
                {
                    case AbpLoginResultType.Success:
                        await _signInManager.SignInAsync(loginResult.Identity, false);
                        //await addUserIdToSession();
                        return returnToHomeURL();
                    case AbpLoginResultType.UnknownExternalLogin:
                        return await RegisterForExternalLogin(externalLoginInfo);
                    default:
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
                            tenancyName
                        );
                }
            }
            else if (externalLoginInfo == null && User.Identity.IsAuthenticated)
            {
                await _signInManager.SignInAsync((ClaimsIdentity)User.Identity, false);
                return returnToHomeURL();
            }*/

            /*var result = await HttpContext.AuthenticateAsync("Temp");
            if (!result.Succeeded) throw new Exception("no external authentication going on right now...");

            var extUser = result.Principal;
            var extUserId = extUser.FindFirst(ClaimTypes.NameIdentifier);
            var issuer = extUserId.Issuer;

            // provisioning logic happens here...

            var claims = new List<Claim>
            {
                new Claim("sub", "123456789"),
                new Claim("name", "Dominick"),
                new Claim("email", extUser.FindFirst(ClaimTypes.Email).Value),
                new Claim("role", "Geek")
            };

            var ci = new ClaimsIdentity(claims, "password", "name", "role");
            var pa = new ClaimsPrincipal(ci);

            await HttpContext.SignInAsync(pa);
            await HttpContext.SignOutAsync("Temp");*/



            return returnToHomeURL();
        }

        private ActionResult returnToHomeURL()
        {
            return Redirect("http://localhost:4200");
        }

        private async Task<ActionResult> RegisterForExternalLogin(List<Claim> claims)
        {
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            var user = await _userRegistrationManager.RegisterAsync(name.Value, String.Empty, email.Value, email.Value, Authorization.Users.User.CreateRandomPassword(), true);

            user.Logins = new List<UserLogin>{
                        new UserLogin
                        {
                            LoginProvider = name.Issuer,
                            ProviderKey = name.Issuer,
                            TenantId = user.TenantId
                        }
                    };

            await CurrentUnitOfWork.SaveChangesAsync();
            var newUserClaims = externalLoginInfo.Principal.Claims.Append(new Claim("userid", user.Id.ToString()));
            
            await _userManager.AddClaimsAsync(user, newUserClaims);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return returnToHomeURL();
        }


    }
}
