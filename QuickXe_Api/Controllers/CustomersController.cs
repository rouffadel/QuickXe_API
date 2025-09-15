using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL;
using DAL.DAO;
using Microsoft.AspNetCore.Authorization;
using DAL.DTO;
using System.Diagnostics.Metrics;

namespace QuickXe_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly OrganizationDbContext _context;

        public CustomersController(OrganizationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }


        //[HttpGet("ByPhoneNumber/{phoneNumber}")]
        //public async Task<IActionResult> GetCodeByPhoneNumber(string phoneNumber)
        //{
        //    var code = await _context.Customers
        //        .Where(a => a.PhoneNumber == phoneNumber)
        //        .Join(_context.CustomerOTPs,
        //              a => a.CustomerId,
        //              s => s.CustomerId,
        //              (a, s) => s.Code)
        //        .FirstOrDefaultAsync();

        //    if (code == null)
        //    {
        //        return NotFound(new { Status = "Error", Message = "Email not found" });
        //    }

        //    // Send the email
        //    //bool emailSent = await SendEmailAsync(email, emailCode);

        //    return Ok(new { Status = "OK", Data = code });


        //    //return Ok(new { Status = "OK", Message = "Email sent successfully" });
            
            
        //}



        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCustomer(string id, Customer customer)
        //{
        //    if (id != customer.CustomerId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(customer).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CustomerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Customer>> PostCustomer(CreateCustomerDTO customer)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Customer customerDetail = new Customer()
        //            {
        //                Name = customer.Name,
        //                PhoneNumber = customer.PhoneNumber,
        //                Email = customer.Email,
        //                Address = customer.Address
        //            };

        //            _context.Customers.Add(customerDetail);
        //            await _context.SaveChangesAsync();
        //            return CreatedAtAction("GetCustomerById", new { id = customerDetail.CustomerId }, customerDetail);
        //        }
        //        else
        //        {
        //            return BadRequest(ModelState);
        //        }
        //    }
        //    catch (Exception E)
        //    {
        //        string msg = "";
        //        if (E.InnerException != null)
        //        {
        //            msg = E.InnerException.Message;
        //        }
        //        else
        //        {
        //            msg = E.Message;
        //        }
        //        return StatusCode(500, msg);
        //    }
        //}




        [HttpPost("register")]
        public async Task<ActionResult<CustomerOTP>> PostCustomer(CreateCustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // First, insert the customer record
                    Customer newCustomer = new Customer
                    {
                        Name = customer.Name,
                        PhoneNumber = customer.PhoneNumber,
                        Email = customer.Email,
                        Address = customer.Address
                    };

                    _context.Customers.Add(newCustomer);
                    await _context.SaveChangesAsync();

                    //// Create a CustomerOTP object
                    //var customerOTPDto = new CreateCustomerOTPDTO
                    //{
                    //    CustomerId = newCustomer.CustomerId, // Assuming response has UserId
                    //};

                    //// Add the CustomerOTP record
                    //CustomerOTP customerOTPDetail = new CustomerOTP()
                    //{
                    //    CustomerId = customerOTPDto.CustomerId,
                    //};

                    //_context.CustomerOTPs.Add(customerOTPDetail);

                    //// Save both changes in the transaction
                    //await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return Ok(new { Status = "OK", Data = newCustomer });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { Status = "Error", Message = ex.Message.ToString() });
                }
            }
        }




        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
