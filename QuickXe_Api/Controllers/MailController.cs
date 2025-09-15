using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DAL.Interface;
using Microsoft.AspNetCore.Authorization;

namespace QuickXe_Api.Controllers
{
    [Route("api/mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] MailParams mailParams)
        {
            if (mailParams == null || string.IsNullOrEmpty(mailParams.ToEmail))
            {
                return BadRequest("Invalid email request.");
            }

            await _mailService.SendEmailAsync(mailParams);
            return Ok("Email sent successfully.");
        }
    }

}
