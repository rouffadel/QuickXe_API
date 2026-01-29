using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Models;
using DAL.Interface;
using Azure;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace QuickXe_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOTPsController : ControllerBase
    {
        private readonly OrganizationDbContext _context;
        private readonly ITwilioService _twilioService;

        public CustomerOTPsController(OrganizationDbContext context, ITwilioService twilioService)
        {
            _context = context;
            _twilioService = twilioService;
        }

        // GET: api/CustomerOTPs
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CustomerOTP>>> GetCustomerOTPs()
        {
            return await _context.CustomerOTPs.ToListAsync();
        }

        // GET: api/CustomerOTPs/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CustomerOTP>> GetCustomerOTP(int id)
        {
            var customerOTP = await _context.CustomerOTPs.FindAsync(id);

            if (customerOTP == null)
            {
                return NotFound();
            }

            return customerOTP;
        }

        // PUT: api/CustomerOTPs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerOTP(int id, CustomerOTP customerOTP)
        {
            if (id != customerOTP.Id)
            {
                return BadRequest();
            }

            _context.Entry(customerOTP).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerOTPExists(id))
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

        // POST: api/CustomerOTPs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //[HttpPost]
        //public async Task<ActionResult<CustomerOTP>> PostCustomerOTP(CustomerOTP customerOTP)
        //{
        //    _context.CustomerOTPs.Add(customerOTP);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCustomerOTP", new { id = customerOTP.Id }, customerOTP);
        //}



        [HttpPost]
        public async Task<IActionResult> PostCustomerOTP(CreateCustomerOTPDTO customerOTP)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if Login (OTPType == 2) and user exists
                    if (customerOTP.OTPType == 2)
                    {
                        var exists = _context.Customers.Any(x => x.PhoneNumber == customerOTP.PhoneNumber);
                        if (!exists)
                        {
                            return NotFound(new { message = "User not registered. Please sign up first." });
                        }
                    }
                    CustomerOTP customerOTPDetail = new CustomerOTP()
                    {
                        PhoneNumber = customerOTP.PhoneNumber,
                        OTPType = customerOTP.OTPType
                    
                    };

                    _context.CustomerOTPs.Add(customerOTPDetail);
                    await _context.SaveChangesAsync();

                    // Send OTP via WhatsApp using Twilio
                    await _twilioService.SendOtpWhatsAppAsync(customerOTPDetail.PhoneNumber, customerOTPDetail.OTP);

                    return CreatedAtAction("GetCustomerOTP", new { id = customerOTPDetail.Id }, customerOTPDetail);
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





        //[HttpPost("CreateOTP")]
        //public async Task<ActionResult<CustomerOTP>> PostCustomerOTP(CustomerOTP customerOTP)
        //{
        //    _context.CustomerOTPs.Add(customerOTP);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCustomerOTP", new { id = customerOTP.Id }, customerOTP);
        //}


        //[HttpPost("validate-otp")]
        //public async Task<IActionResult> ValidateOTP([FromBody] OTPRequest request)
        //{
        //    if (request == null || string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.OTP))
        //    {
        //        return BadRequest(new { message = "Invalid request data." });
        //    }

        //    var code = _context.CustomerOTPs.Select(cd => cd.Code).FirstOrDefaultAsync();

        //    // Retrieve the OTP from the database using the provided Code
        //    var storedOtp = _context.CustomerOTPs
        //                              .Where(c => c.Code == request.Code)
        //                              .Select(c => c.OTP)
        //                              .FirstOrDefaultAsync();

        //    // Check if OTP exists for the given Code
        //    if (storedOtp == null)
        //    {
        //        return NotFound(new { message = "Code not found." });
        //    }

        //    // Compare OTP values
        //    if (storedOtp == request.OTP)
        //    {
        //        //return Ok(new { message = "OTP is valid." });
        //        return Ok(new { Status = "OK", Data = code });

        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Invalid OTP." });
        //    }
        //}





        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OTP))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            // Note: In the registration flow, the customer is created BEFORE OTP validation.
            // So we should NOT block if the phone number exists. We should proceed to validate the OTP.

            var phoneNumber = request.PhoneNumber.Trim();
            // Get the most recent OTP for this phone number (any type)
            var latestOtp = await _context.CustomerOTPs
                                          .Where(c => c.PhoneNumber.Trim() == phoneNumber)
                                          .OrderByDescending(c => c.CreatedOn)
                                          .Select(c => new { c.OTP, c.ExpireOn, c.OTPType })
                                          .FirstOrDefaultAsync();

            // Check if OTP exists
            if (latestOtp == null)
            {
                return NotFound(new { message = "No OTP found. Please request a new OTP." });
            }

            // Check if the OTP is expired
            if (latestOtp.ExpireOn < DateTime.Now)
            {
                return BadRequest(new { message = "OTP has expired. Please request a new OTP." });
            }

            // Compare OTP values
            if (latestOtp.OTP.Trim() == request.OTP.Trim())
            {
                // OTP is valid. Fetch customer data to allow Direct Login.
                var customer = await _context.Customers
                                            .Where(c => c.PhoneNumber == request.PhoneNumber)
                                            .Select(c => new { c.CustomerId, c.Name, c.Email, c.PhoneNumber })
                                            .FirstOrDefaultAsync();

                if (customer != null)
                {
                    // Return OK status and Customer Data for session storage in frontend
                    return Ok(new { Status = "OK", Message = "OTP Verified Successfully!", Data = customer });
                }
                else
                {
                    // Fallback if customer not found (should not happen in standard signup flow)
                    return Ok(new { Status = "OK", Message = "OTP Verified!", Data = new { PhoneNumber = request.PhoneNumber } });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid OTP. Please check and try again." });
            }
        }


        //[HttpPost("validate-otp")]
        //public async Task<IActionResult> ValidateOTP([FromBody] OTPForLoginRequestDTO request)
        //{
        //    if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OTP))
        //    {
        //        return BadRequest(new { message = "Invalid request data." });
        //    }

        //    // Check if the phone number exists in the customers table
        //    bool phoneExists = await _context.Customers.AnyAsync(p => p.PhoneNumber == request.PhoneNumber);
        //    if (!phoneExists)
        //    {
        //        return NotFound(new { message = "Phone number not found." });
        //    }


        //    // Retrieve the stored OTP and its expiration time
        //    var otpEntry = await _context.CustomerLoginOTPs
        //        .Where(c => c.PhoneNumber == request.PhoneNumber)
        //        //Select(c => new { c.OTP, c.ExpireOn })
        //        .Select(c => new { c.OTP, c.ExpireOn })
        //        .FirstOrDefaultAsync();

        //    // Check if OTP exists
        //    if (otpEntry == null || string.IsNullOrEmpty(otpEntry.OTP))
        //    {
        //        return BadRequest(new { message = "Invalid credentials." });
        //    }

        //    // Check if the OTP is expired
        //    if (otpEntry.ExpireOn < DateTime.Now)
        //    {
        //        return BadRequest(new { message = "OTP has expired." });
        //    }

        //    // Check if the OTP is expired
        //    //if (otpEntry.ExpiryTime < DateTime.UtcNow)
        //    //{
        //    //    return BadRequest(new { message = "OTP has expired." });
        //    //}

        //    // Compare OTP values
        //    if (otpEntry.OTP != request.OTP)
        //    {
        //        return BadRequest(new { message = "Invalid credentials." });
        //    }

        //    return Ok(new { Status = "OK" });
        //}



        [HttpPost("validate-login-otp")]
        public async Task<IActionResult> ValidateLoginOTP([FromBody] OTPRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OTP))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            // Check if the phone number exists in the customers table
            bool phoneExists = await _context.Customers.AnyAsync(p => p.PhoneNumber == request.PhoneNumber);
            if (!phoneExists)
            {
                return NotFound(new { message = "Phone number not registered. Please sign up first." });
            }

            var phoneNumber = request.PhoneNumber.Trim();
            // Get the most recent OTP for this phone number (any type)
            var latestOtp = await _context.CustomerOTPs
                                          .Where(c => c.PhoneNumber.Trim() == phoneNumber)
                                          .OrderByDescending(c => c.CreatedOn)
                                          .Select(c => new { c.OTP, c.ExpireOn, c.OTPType, c.CreatedOn })
                                          .FirstOrDefaultAsync();

            // Check if OTP exists
            if (latestOtp == null)
            {
                return NotFound(new { message = "No OTP found. Please request a new OTP." });
            }

            var response = await _context.Customers
                                        .Where(b => b.PhoneNumber == request.PhoneNumber)
                                        .Select(d => new { d.CustomerId, d.Name, d.Email })
                                        .FirstOrDefaultAsync();

            // Check if the OTP is expired
            if (latestOtp.ExpireOn < DateTime.Now)
            {
                return BadRequest(new { 
                    message = "OTP has expired. Please request a new OTP.",
                    expiredAt = latestOtp.ExpireOn,
                    currentTime = DateTime.Now
                });
            }

            // Compare OTP values
            if (latestOtp.OTP.Trim() == request.OTP.Trim())
            {
                return Ok(new { Status = "OK", Message = "Login Successful!", Data = response });
            }
            else
            {
                return BadRequest(new { message = "Invalid OTP. Please check and try again." });
            }
        }



        // DELETE: api/CustomerOTPs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerOTP(int id)
        {
            var customerOTP = await _context.CustomerOTPs.FindAsync(id);
            if (customerOTP == null)
            {
                return NotFound();
            }

            _context.CustomerOTPs.Remove(customerOTP);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerOTPExists(int id)
        {
            return _context.CustomerOTPs.Any(e => e.Id == id);
        }
    }
}
