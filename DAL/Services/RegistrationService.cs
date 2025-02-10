using DAL.DAO;
using DAL.Exceptions;
using DAL.Interface;
using DAL.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly OrganizationDbContext _context;

        public RegistrationService( IConfiguration configuration, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, OrganizationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<RegistrationResponse> Register(RegistrationRequest model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                RegistrationResponse registrationResponse = new RegistrationResponse();

                if (existingUser == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        //PhoneNumber = model.MobileNumber,
                        ContactName = $"{model.FirstName} {model.LastName}",
                        //ContactNo = model.MobileNumber,
                        CreateDate = DateTime.Now,
                        IsApproved = false,
                        CompanyName = model.CompanyName,
                    };

                    var result = await _userManager.CreateAsync(newUser, model.Password);

                    if (result.Succeeded)
                    {
                        // await CreateOtpValidity(model.Email);
                        //var roleResult = await _userManager.AddToRolesAsync(newUser, new List<string> { model.RoleName });
                        var roleResult = await _userManager.AddToRolesAsync(newUser, new List<string> { model.RoleName });
                        registrationResponse.Email = newUser.Email;
                        registrationResponse.Status = true;
                        registrationResponse.Message = "User created successfully!";
                    }
                    else
                    {
                        registrationResponse.Status = false;
                        registrationResponse.Message = "Error occurred while creating the user!";
                    }
                }
                else
                {
                    registrationResponse.Status = false;
                    registrationResponse.Message = "User already exists!";
                }

                return registrationResponse;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return new RegistrationResponse
                {
                    Status = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }


        public async Task<LoginResponse> Login(LoginRequest model)
        {
            try
            {
                ValidateModel(model);

                var user = await _userManager.FindByEmailAsync(model.Username);
                LoginResponse loginResponse = new LoginResponse();

                if (user != null)
                {
                    if (await IsUserLockedoutAsync(user))
                        throw new PortalException($"Too many failed attempts to login. Try again in {DateTimeOffset.UtcNow - user.LockoutEnd}");

                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        JwtSecurityToken rawToken = await CreateJwtSecurityTokenAsync(user,
                            _configuration["AppSettings:ValidIssuer"],
                            _configuration["AppSettings:ValidAudience"],
                            _configuration["AppSettings:Secret"]);

                        string token = new JwtSecurityTokenHandler().WriteToken(rawToken);

                        var userRoles = await _userManager.GetRolesAsync(user);
                        List<Roles> rolesIds = new List<Roles>();

                        foreach (var userRole in userRoles)
                        {
                            var roles = await _roleManager.FindByNameAsync(userRole);
                            if (roles != null)
                            {
                                Roles role = new Roles
                                {
                                    RoleName = roles.Name
                                };
                                rolesIds.Add(role);
                            }
                        }

                        loginResponse.Username = user.UserName;
                        loginResponse.UserId = user.Id;
                        loginResponse.Email = user.Email;
                        loginResponse.ValidTo = rawToken.ValidTo;
                        loginResponse.Token = token;
                        loginResponse.Roles = rolesIds;
                        loginResponse.Status = true;
                        loginResponse.Message = "User exists";
                    }
                    else
                    {
                        loginResponse.Status = false;
                        loginResponse.Message = "Invalid user";
                    }
                }
                else
                {
                    loginResponse.Status = false;
                    loginResponse.Message = "User doesn't exist";
                }
                return loginResponse;
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Status = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        private static void ValidateModel(LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                throw new DataValidationException("Please enter user name");
            }
            else if (string.IsNullOrEmpty(model.Password))
            {
                throw new DataValidationException("Please enter password");
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
                expires: DateTime.Now.AddDays(30),
                claims: identityClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256));

            return rawToken;
        }
       

        public string GenerateStringOTP()
        {
            const string digits = "0123456789";
            Random random = new Random();
            int otpLength = 6;
            string otpString = new string(Enumerable.Repeat(digits, otpLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return otpString;
        }



        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}