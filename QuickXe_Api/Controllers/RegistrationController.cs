using DAL;
using DAL.DAO;
using DAL.Exceptions;
using DAL.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginRequest = DAL.Models.LoginRequest;
using ResetPasswordRequest = DAL.Models.ResetPasswordRequest;

namespace QuickXe_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registration;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationRole> _roleManager;
        private readonly OrganizationDbContext _context;


        public RegistrationController(IRegistrationService registration, IConfiguration configuration, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, OrganizationDbContext context)
        {
            _registration = registration;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;


        }

        //// GET: api/Registration
        //[HttpGet("registered_user")]
        //public async Task<IActionResult> GetApplicationUser()
        //{
        //    var tenants = await _context.ApplicationUser.ToListAsync();
        //    return Ok(tenants);
        //}


        // GET: api/Registration
        [Authorize]
        [HttpGet("registered_user")]
        public async Task<IActionResult> GetApplicationUser()
        {
            var tenants = await _context.ApplicationUser
                .Select(user => new
                {
                    user.ContactName,
                    user.Email,
                    user.ContactNo,
                    user.CompanyName
                })
                .ToListAsync();

            return Ok(tenants);
        }





        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _registration.Register(model);
                return Ok(new { Status = "OK", Data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message.ToString() });

            }
        }

        private static void ValidateModel(LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                throw new DataValidationException("Please Enter User Name");
            }
            else if (string.IsNullOrEmpty(model.Password))
            {
                throw new DataValidationException("Please Enter Password");
            }
        }
        private async Task<bool> IsUserLockedoutAsync(ApplicationUser user)
        {
            var now = DateTimeOffset.UtcNow;
            if (user.LockoutEnd is null)
            {
                user.LockoutEnd = now;
                await _userManager.SetLockoutEndDateAsync(user, now);
            }
            if (!user.LockoutEnabled && user.LockoutEnd >= now)
            {
                user.LockoutEnabled = true;
                await _userManager.SetLockoutEnabledAsync(user, true);
            }
            if (user.LockoutEnabled)
            {
                if (user.LockoutEnd <= now)
                {
                    user.LockoutEnabled = false;
                    await _userManager.SetLockoutEnabledAsync(user, false);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<JwtSecurityToken> CreateJwtSecurityTokenAsync(ApplicationUser user, string issuer, string audience, string secret)
        {

            List<Claim> identityClaims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            foreach (string roleName in userRoles)
                identityClaims.Add(new Claim(ClaimTypes.Role, roleName));


            JwtSecurityToken rawToken = new(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddHours(3),
                claims: identityClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256));

            return rawToken;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                var response = await _registration.Login(model);
                return Ok(new { Status = "OK", Data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message.ToString() });

            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            try
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    return BadRequest("New password and confirm password do not match.");
                }

                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user == null)
                {
                    throw new DataNotFoundException("User not found.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok("Password reset successfully.");
                }
                else
                {
                    throw new Exception("Failed to reset password.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message.ToString() });
            }
        }

       
       

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.userName);
            JwtSecurityToken rawToken = await CreateJwtSecurityTokenAsync(user,
                     _configuration["AppSettings:ValidIssuer"],
                     _configuration["AppSettings:ValidAudience"],
                     _configuration["AppSettings:Secret"]);

            string token = new JwtSecurityTokenHandler().WriteToken(rawToken);
            await _context.SaveChangesAsync();
            return Ok(new { Token = token });
        }

       

    }
}