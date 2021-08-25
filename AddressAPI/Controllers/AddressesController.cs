using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressAPI.DAL;
using AddressAPI.Model;

namespace AddressAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AddressesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        /// <summary>
        /// Gets all the addresses in the database.
        /// </summary>
        /// <returns>A list of Address objects.</returns>
        /// <response code="200">The addresses were found.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        // GET: api/Addresses/5
        /// <summary>
        /// Gets a specific address from the database.
        /// </summary>
        /// <param name="id">The id of the requested address.</param>
        /// <returns>An Address object.</returns>
        /// <response code="200">The address was found.</response>
        /// <response code="404">No address was found with that id.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        /// <summary>
        /// Updates an address in the database.
        /// </summary>
        /// <param name="id">The id of the to-be updated address.</param>
        /// <param name="address">The address to be saved in the database.</param>
        /// <returns>A status code.</returns>
        /// <response code="400">The entered id does not correspond to the id of the object.</response>
        /// <response code="404">No address was found with that id.</response>
        /// <response code="204">The address was updated in the database.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressId)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        // POST: api/Addresses
        /// <summary>
        /// Saves an address in the database.
        /// </summary>
        /// <param name="address">The to-be saved address.</param>
        /// <returns>A status code with the address.</returns>
        /// <response code="201">The address was saved in the database.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        /// <summary>
        /// Deletes an address from the database.
        /// </summary>
        /// <param name="id">The id of the to-be deleted address.</param>
        /// <returns>A status code.</returns>
        /// <response code="404">No address was found with that id.</response>
        /// <response code="204">The address was deleted.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}
