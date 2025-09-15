using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL;
using DAL.DAO;
using DAL.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace QuickXe_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailsController : ControllerBase
    {
        private readonly OrganizationDbContext _context;

        public SendEmailsController(OrganizationDbContext context)
        {
            _context = context;
        }

        // GET: api/SendEmails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendEmail>>> GetSendEmails()
        {
            return await _context.SendEmails.ToListAsync();
        }

        // GET: api/SendEmails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SendEmail>> GetSendEmail(int id)
        {
            var sendEmail = await _context.SendEmails.FindAsync(id);

            if (sendEmail == null)
            {
                return NotFound();
            }

            return sendEmail;
        }


        [HttpGet("GetUserName/{emailcode}")]
        public async Task<ActionResult<SendEmail>> GetUserNameByEmailCode(string emailcode)
        {
            var userName = _context.SendEmails
                .Where(s => s.EmailCode == emailcode)
                .Join(_context.ApplicationUser,
                         s => s.UserId,
                         a => a.Id,
                         (s, a) => a.UserName)
                .FirstOrDefault();

            return Ok(new { Status = "OK", Data = userName });
        }


        //[HttpGet("ByUser/{userId}")]
        //public async Task<IActionResult> GetEmailCodeByUserId(string userId)
        //{
        //    var emailCode = await _context.SendEmails
        //        .Where(c => c.UserId == userId)
        //        .FirstOrDefaultAsync();

        //    if (emailCode == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(emailCode);
        //}



        // PUT: api/SendEmails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSendEmail(int id, SendEmail sendEmail)
        {
            if (id != sendEmail.Id)
            {
                return BadRequest();
            }

            _context.Entry(sendEmail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SendEmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SendEmails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //[HttpPost]
        //public async Task<ActionResult<SendEmail>> PostSendEmail(SendEmail sendEmail)
        //{
        //    _context.SendEmails.Add(sendEmail);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetSendEmail", new { id = sendEmail.Id }, sendEmail);
        //}



        [HttpPost]
        public async Task<ActionResult<SendEmail>> PostSendEmail(CreateSendEmailDTO sendEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SendEmail sendEmailDetail = new SendEmail()
                    {
                        //EmailCode = sendEmail.EmailCode,
                        UserId = sendEmail.UserId,
                    };

                    _context.SendEmails.Add(sendEmailDetail);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetSendEmail", new { id = sendEmailDetail.Id }, sendEmailDetail);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception E)
            {
                string msg = "";
                if (E.InnerException != null)
                {
                    msg = E.InnerException.Message;
                }
                else
                {
                    msg = E.Message;
                }
                return StatusCode(500, msg);
            }
        }



        // DELETE: api/SendEmails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSendEmail(int id)
        {
            var sendEmail = await _context.SendEmails.FindAsync(id);
            if (sendEmail == null)
            {
                return NotFound();
            }

            _context.SendEmails.Remove(sendEmail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SendEmailExists(int id)
        {
            return _context.SendEmails.Any(e => e.Id == id);
        }
    }
}
