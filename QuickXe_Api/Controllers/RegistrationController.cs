using Azure;
using DAL;
using DAL.DAO;
using DAL.DTO;
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
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginRequest = DAL.Models.LoginRequest;
using ResetPasswordRequest = DAL.Models.ResetPasswordRequest;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace QuickXe_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegistrationController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IRegistrationService _registration;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationRole> _roleManager;
        private readonly OrganizationDbContext _context;


        public RegistrationController(IWebHostEnvironment webHostEnvironment, IRegistrationService registration, IConfiguration configuration, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, OrganizationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;

            _registration = registration;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;


        }

     
        [HttpGet("registereduser")]
        public async Task<IActionResult> GetApplicationUser()
        {
            var admin = await _context.ApplicationRoles
                .Where(x => x.Name == "Admin")
                .FirstOrDefaultAsync(); // Gets the first matching role or null

            var tenants = await _context.ApplicationUser
                .Where(user => user.RoleId != admin.Id)
                .Select(user => new
                {
                    user.ContactName,
                    user.Email,
                    user.ContactNo,
                    user.CompanyName,
                    user.EmailStatus,
                    user.IsActive,
                    user.CreateDate,
                    user.ActivationDate
                })
                .ToListAsync();

            return Ok(tenants);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.ApplicationUser.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new
            {
                user.ContactName,
                user.Email,
                user.ContactNo,
                user.CompanyName,
                user.Country,
                user.State,
                user.District,
                user.Latitude,
                user.Longitude
            };

            return Ok(userDto);
        }



        [HttpGet("ByUser/{email}")]
        public async Task<IActionResult> GetEmailCodeByEmailId(string email)
        {
            var emailCode = await _context.ApplicationUser
                .Where(a => a.Email == email)
                .Join(_context.SendEmails,
                      a => a.Id,
                      s => s.UserId,
                      (a, s) => s.EmailCode)
                .FirstOrDefaultAsync();

            if (emailCode == null)
            {
                return NotFound(new { Status = "Error", Message = "Email not found" });
            }

            // Send the email
            bool emailSent = await SendEmailAsync(email, emailCode);

            if (emailSent)
            {
                return Ok(new { Status = "OK", Message = "Email sent successfully" });
            }
            else
            {
                return StatusCode(500, new { Status = "Error", Message = "Failed to send email" });
            }
        }

    
        private async Task<bool> SendEmailAsync(string toEmail, string emailCode)
        {
            try
            {
                string subject = "Registration Successful!!!";

                // Get the absolute path of the email template
                string templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Templates", "EmailTemplate.html");

                if (!System.IO.File.Exists(templatePath))
                {
                    return false; // File not found
                }

                // Read the email template file
                string body = await System.IO.File.ReadAllTextAsync(templatePath, Encoding.UTF8);

                // Replace placeholder with actual reset password link
                string resetPasswordTemplate = _configuration["AppSettings:ResetPasswordUrl"];
                string resetPasswordLink = resetPasswordTemplate.Replace("{code}", emailCode);

                body = body.Replace("{{ResetPasswordLink}}", resetPasswordLink);

                string key = _configuration["SmtpSettings:Password"];
                string host = _configuration["SmtpSettings:Host"]; 
                string port = _configuration["SmtpSettings:Port"];
                string userName = _configuration["SmtpSettings:UserName"];


                using (var client = new SmtpClient(host))
                {
                    client.Port = int.Parse(port);
                    client.Credentials = new NetworkCredential(userName, key);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(userName, "QuickXe Support"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }



        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegistrationRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    using (var transaction = await _context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // First, insert the registration record
        //            var response = await _registration.Register(model);

        //            // Create a SendEmail object
        //            var sendEmailDto = new CreateSendEmailDTO
        //            {
        //                UserId = response.UserId, // Assuming response has UserId
        //            };

        //            // Add the SendEmail record
        //            SendEmail sendEmailDetail = new SendEmail()
        //            {
        //                UserId = sendEmailDto.UserId,
        //            };

        //            _context.SendEmails.Add(sendEmailDetail);

        //            // Save both changes in the transaction
        //            await _context.SaveChangesAsync();

        //            // Commit transaction
        //            await transaction.CommitAsync();

        //            return Ok(new { Status = "OK", Data = response });
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return BadRequest(new { Status = "Error", Message = ex.Message.ToString() });
        //        }
        //    }
        //}



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Create user using Identity (handles transaction internally)
                var response = await _registration.Register(model);

                if (!response.Status)
                    return BadRequest(new { Status = "Error", Message = response.Message });

                // Insert SendEmail separately
                var sendEmailDetail = new SendEmail
                {
                    UserId = response.UserId
                };

                _context.SendEmails.Add(sendEmailDetail);
                await _context.SaveChangesAsync();

                return Ok(new { Status = "OK", Data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }



        [HttpPut("updateuser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO model)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            // Update only the specified fields
            user.ContactName = model.ContactName;
            user.ContactNo = model.ContactNo;
            user.CompanyName = model.CompanyName;
            user.Country = model.Country;
            user.State = model.State;
            user.District = model.District;
            user.Latitude = model.Latitude;
            user.Longitude = model.Longitude;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Status = "OK", Data = "User updated successfully." });
        }


        [HttpPut("updateisactive/{username}")]
        public async Task<IActionResult> UpdateIsActive(string username, [FromBody] UpdateIsActiveDTO model)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("User name is required.");

            //var user = await _userManager.FindByIdAsync(id);
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
                return NotFound("User not found.");

            // Update only the specified fields
            user.IsActive = model.IsActive;
            user.ActivationDate = model.ActivationDate;
            //user.ContactName = model.ContactName;
            //user.ContactNo = model.ContactNo;
            //user.CompanyName = model.CompanyName;
            //user.Country = model.Country;
            //user.State = model.State;
            //user.District = model.District;
            //user.Latitude = model.Latitude;
            //user.Longitude = model.Longitude;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Status = "OK", Data = "IsActive updated successfully." });
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


    //    private async Task<JwtSecurityToken> CreateJwtSecurityTokenAsync(ApplicationUser user, string issuer, string audience, string secret)
    //    {
    //        // Generate claims for the user
    //        List<Claim> identityClaims = new()
    //{
    //    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // iat
    //    new Claim(JwtRegisteredClaimNames.Iss, issuer), // iss
    //    new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // exp
    //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User's unique identifier
    //    new Claim(ClaimTypes.Name, user.UserName) // User's username
    //};

    //        // Add roles to the claims if the user has any
    //        IList<string> userRoles = await _userManager.GetRolesAsync(user);
    //        foreach (string roleName in userRoles)
    //        {
    //            identityClaims.Add(new Claim(ClaimTypes.Role, roleName));
    //        }

    //        // Create the JWT
    //        JwtSecurityToken token = new(
    //            issuer: issuer,
    //            audience: audience,
    //            claims: identityClaims,
    //            expires: DateTime.UtcNow.AddHours(3), // Token expiration time
    //            signingCredentials: new SigningCredentials(
    //                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
    //                SecurityAlgorithms.HmacSha256) // Signing algorithm
    //        );

    //        return token;
    //    }



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
                    return Ok(new { Status = "OK", Data = "Reset Successfully" });
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