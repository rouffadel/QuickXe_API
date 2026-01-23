using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DAL;
using DAL.DAO;
using DAL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CountryAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
       private readonly OrganizationDbContext _context;

        public CountriesController(OrganizationDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        //[HttpGet]
        //public async Task<IActionResult> GetCountries()
        //{
        //    var countries = await _context.Countries.ToListAsync();
        //    return Ok(countries);
        //}


        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries
                .Select(country => new
                {
                    country.CountryName,
                    country.CountryId,
                    country.CountryCode,
                    country.CurrencyName,
                    country.CurrencyCode,
                    country.BuyRate,
                    country.SellRate,
                    country.CurrencyAvailable,
                    country.CurrencySymbol,
                    country.TenantId,
                    TenantName = _context.ApplicationUser
                        .Where(user => user.Id == country.TenantId)
                        .Select(user => user.ContactName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(countries);
        }



        // GET: api/Countries/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }



        //// GET: api/Countries/{TenantId}
        //[HttpGet("{TenantId}")]
        //public async Task<IActionResult> GetCountryByTenantId(int id)
        //{
        //    var country = await _context.Countries.FindAsync(id);

        //    if (country == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(country);
        //}


        //// GET: api/Countries/ByTenant/{TenantId}
        //[HttpGet("ByTenant/{tenantId}")]
        //public async Task<IActionResult> GetCountryByTenantId(string tenantId)
        //{
        //    var country = await _context.Countries
        //        .Where(c => c.TenantId == tenantId)
        //        .FirstOrDefaultAsync();

        //    if (country == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(country);
        //}

        // GET: api/Countries/ByTenant/{TenantId}
        [HttpGet("ByTenant/{tenantId}")]
        public async Task<IActionResult> GetCountryByTenantId(string tenantId)
        {
            var country = await _context.Countries
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();

            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }


        // POST: api/Countries
        [HttpPost]
        //public async Task<IActionResult> PostCountry(CreateCountryDTO country)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Country countryDetail = new Country()
        //            {
        //                CountryName = country.CountryName,
        //                CountryCode = country.CountryCode,
        //                CurrencyName = country.CurrencyName,
        //                BuyRate = country.BuyRate,
        //                SellRate = country.SellRate,
        //                TenantId = country.TenantId,
        //            };

        //            _context.Countries.Add(countryDetail);
        //            await _context.SaveChangesAsync();
        //            return CreatedAtAction("GetCountryById", new { id = countryDetail.CountryId }, countryDetail);
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


        public async Task<IActionResult> PostCountry(CreateCountryDTO country)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ✅ Check if country already exists (case-insensitive)
                var existingCountry = await _context.Countries
                    .AnyAsync(c =>
                        c.CountryName.ToLower() == country.CountryName.ToLower() &&
                        c.TenantId == country.TenantId
                    );

                if (existingCountry)
                {
                    return Conflict(new
                    {
                        message = $"Country '{country.CountryName}' already exists."
                    });
                }

                // Auto-populate CurrencySymbol and CurrencyCode from CountriesMaster if not provided
                string finalCurrencySymbol = country.CurrencySymbol;
                string finalCurrencyCode = country.CurrencyCode;
                if (!string.IsNullOrEmpty(country.CountryCode))
                {
                    string searchCode = country.CountryCode.Trim().ToLower();
                    var masterCountry = await _context.CountriesMaster
                        .FirstOrDefaultAsync(cm => cm.CountryCode != null && cm.CountryCode.Trim().ToLower() == searchCode);
                    
                    if (masterCountry != null)
                    {
                        if (string.IsNullOrEmpty(finalCurrencySymbol))
                        {
                            finalCurrencySymbol = masterCountry.CurrencySymbol;
                        }
                        if (string.IsNullOrEmpty(finalCurrencyCode))
                        {
                            finalCurrencyCode = masterCountry.CurrencyCode;
                        }
                    }
                }

                Country countryDetail = new Country()
                {
                    CountryName = country.CountryName,
                    CountryCode = country.CountryCode,
                    CurrencyName = country.CurrencyName,
                    CurrencyCode = finalCurrencyCode,
                    BuyRate = country.BuyRate,
                    SellRate = country.SellRate,
                    CurrencyAvailable = country.CurrencyAvailable,
                    CurrencySymbol = finalCurrencySymbol,
                    TenantId = country.TenantId,
                };

                _context.Countries.Add(countryDetail);
                await _context.SaveChangesAsync();
                
                // Return the complete object including the potentially auto-populated symbol
                return CreatedAtAction(
                    "GetCountryById",
                    new { id = countryDetail.CountryId },
                    countryDetail
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }


        // PUT: api/Countries/{id}
        [HttpPut("{id}")]

        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.CountryId)
            {
                return BadRequest();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(country).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.CountryId == id);
        }
    }
}
