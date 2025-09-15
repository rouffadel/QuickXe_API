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

namespace QuickXe_Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesMastersController : ControllerBase
    {
        private readonly OrganizationDbContext _context;

        public CountriesMastersController(OrganizationDbContext context)
        {
            _context = context;
        }

        // GET: api/CountriesMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountriesMaster>>> GetCountriesMaster()
        {
            return await _context.CountriesMaster.ToListAsync();
        }

        // GET: api/CountriesMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountriesMaster>> GetCountriesMaster(int id)
        {
            var countriesMaster = await _context.CountriesMaster.FindAsync(id);

            if (countriesMaster == null)
            {
                return NotFound();
            }

            return countriesMaster;
        }

        // PUT: api/CountriesMasters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountriesMaster(int id, CountriesMaster countriesMaster)
        {
            if (id != countriesMaster.CountryId)
            {
                return BadRequest();
            }

            _context.Entry(countriesMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountriesMasterExists(id))
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

        // POST: api/CountriesMasters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountriesMaster>> PostCountriesMaster(CountriesMaster countriesMaster)
        {
            _context.CountriesMaster.Add(countriesMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountriesMaster", new { id = countriesMaster.CountryId }, countriesMaster);
        }

        // DELETE: api/CountriesMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountriesMaster(int id)
        {
            var countriesMaster = await _context.CountriesMaster.FindAsync(id);
            if (countriesMaster == null)
            {
                return NotFound();
            }

            _context.CountriesMaster.Remove(countriesMaster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountriesMasterExists(int id)
        {
            return _context.CountriesMaster.Any(e => e.CountryId == id);
        }
    }
}
