using DAL.DAO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
namespace QuickXe_Api.SecurityProvider
{
    public class SecurityPinProvider : ApplicationTotpSecurityPinProvider<ApplicationUser>
    {
        public static readonly string TotpProvider = "TotpSecurityPinProvider";
        public static readonly string EmailProvider = "EmailSecurityPinProvider";
        public static readonly string PhoneProvider = "PhoneSecurityPinProvider";
        public static readonly string PasswordResetProvider = "PasswordResetSecurityPinProvider";
    }

    public class ApplicationTotpSecurityPinProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user) => false;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var token = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var code = Rfc6238AuthenticationService.GenerateCode(token, modifier, 6);
            return $"{code:000000}";
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (!Int32.TryParse(token, out int code))
                return false;
            var securityToken = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
            var modifier = await GetUserModifierAsync(purpose, manager, user);
            var valid = Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier, token.Length);
            return valid;
        }

        public override Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return base.GetUserModifierAsync(purpose, manager, user);
        }
    }
}
