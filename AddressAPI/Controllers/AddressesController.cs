using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressAPI.DAL;
using AddressAPI.Model;
using AddressAPI.Controllers.Enums;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

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
        /// <response code="204">The address was updated in the database.</response>
        /// <response code="400">The entered id does not correspond to the id of the object.</response>
        /// <response code="404">No address was found with that id.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
        /// <response code="204">The address was deleted.</response>
        /// <response code="404">No address was found with that id.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
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

        // GET: api/Addresses/Search?column=AddressId&comparator=1&order=AddressId
        /// <summary>
        /// Search through the database.
        /// </summary>
        /// <param name="column">What column to search through.</param>
        /// <param name="comparator">What to compare the searched column to.</param>
        /// <param name="order">What to order the results by.</param>
        /// <returns>A list of addresses matching the entered search query.</returns>
        /// <response code="200">The query was executed succesfully and results were found.</response>
        /// <response code="400">The query could not be executed due to one or more parameters being null.</response>
        /// <response code="404">The query was executed succesfully but no results were found.</response>
        [HttpGet("Search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Address>>> SearchDatabase([FromQuery] string column, [FromQuery] string comparator, [FromQuery] string order)
        {
            if (column == null || order == null) return BadRequest();
            _ = Enum.TryParse(column.ToLower(), out Column enumColumn);
            _ = Enum.TryParse(order.ToLower(), out Column orderColumn);
            List<Address> foundAddresses = await _context.Addresses.FromSqlRaw($"SELECT * FROM Address WHERE {enumColumn} == '{comparator}' ORDER BY {orderColumn}").ToListAsync();
            return foundAddresses.Any() ? Ok(foundAddresses) : NotFound();
        }

        // GET: api/Addresses/Distance?addressId1=1&addressId2=2
        /// <summary>
        /// Calculate the distance between two addresses.
        /// </summary>
        /// <param name="addressId1">The id of the first address.</param>
        /// <param name="addressId2">The id of the second address.</param>
        /// <returns>The distance in kilometres as a string.</returns>
        /// <response code="200">The distance was calculated.</response>
        /// <response code="404">One or both of the addresses were not found.</response>
        [HttpGet("Distance")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<String>> CalculateDistance(int addressId1, int addressId2)
        {
            Address address1 = await _context.Addresses.FindAsync(addressId1);
            Address address2 = await _context.Addresses.FindAsync(addressId2);
            if (address1 == null || address2 == null) return NotFound();

            const string apiPath = "http://api.positionstack.com/v1/forward?access_key=36785081c1ae71ced578d341be41afa8&query=";
            HttpClient client = new();
            var jsonAddress1String = await client.GetStringAsync($"{apiPath}{address1.GetAddress()}");
            var jsonAddress2String = await client.GetStringAsync($"{apiPath}{address2.GetAddress()}");

            dynamic jsonAddress1 = JObject.Parse(jsonAddress1String);
            dynamic jsonAddress2 = JObject.Parse(jsonAddress2String);

            double latitudeAddress1 = jsonAddress1.data[0].latitude;
            double latitudeAddress2 = jsonAddress2.data[0].latitude;
            double longitudeAddress1 = jsonAddress1.data[0].longitude;
            double longitudeAddress2 = jsonAddress2.data[0].longitude;

            return Ok($"{CalculateDifferenceBetweenCoords(latitudeAddress1, longitudeAddress1, latitudeAddress2, longitudeAddress2):0.##} km");
        }

        /// <summary>
        /// Calculates the difference between two coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of the first coordinate.</param>
        /// <param name="long1">Longitude of the first coordinate.</param>
        /// <param name="lat2">Latitude of the second coordinate.</param>
        /// <param name="long2">Longitude of the second coordinate.</param>
        /// <returns>The distance in kilometres as a double.</returns>
        private static double CalculateDifferenceBetweenCoords(double lat1, double long1, double lat2, double long2)
        {
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (long2 - long1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) * Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var b = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return b * 6375.5;
        }

        /// <summary>
        /// Checks if an address exists with the given id.
        /// </summary>
        /// <param name="id">The id of the address.</param>
        /// <returns>True when the address exists, otherwise false.</returns>
        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}
