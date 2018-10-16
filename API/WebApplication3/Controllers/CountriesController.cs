using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class CountriesController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();

        // GET: api/Countries
        public IEnumerable<Country> GetCountries()
        {
            return db.Country.ToList().FindAll(c => c.ID != c.Confederation);
        }

        // GET: api/Countries/id
        public Country GetCountryById(int id)
        {
            //return db.Country.Where(c => c.ID == id);
            Country country = new Country();

            try
            {
                country = db.Country.ToList().Find(c => c.ID == id);
            }
            catch
            {
                country.ID = 0;
            }

            return country;
        }

        // GET: api/Countries/Confederation/{id}
        [Route("api/Countries/Confederation/{id}")]
        public IEnumerable<Country> GetCountriesByConfederation(int id)
        {
            try
            {
                return db.Country.ToList().FindAll(c => c.Confederation == id && c.ID != c.Confederation);
            }
            catch
            {
                return new List<Country>();
            }
        }

        #region Auto generated methods
        /*
        // PUT: api/Countries/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCountry(int id, Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != country.ID)
            {
                return BadRequest();
            }

            db.Entry(country).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Countries
        [ResponseType(typeof(Country))]
        public async Task<IHttpActionResult> PostCountry(Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Country.Add(country);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = country.ID }, country);
        }

        // DELETE: api/Countries/5
        [ResponseType(typeof(Country))]
        public async Task<IHttpActionResult> DeleteCountry(int id)
        {
            Country country = await db.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            db.Country.Remove(country);
            await db.SaveChangesAsync();

            return Ok(country);
        }

        private bool CountryExists(int id)
        {
            return db.Country.Count(e => e.ID == id) > 0;
        }*/
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }        
    }
}