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
        public IQueryable<Country> GetCountry()
        {
            return db.Country;
        }

        // GET: api/Countries/id
        public IQueryable<Country> GetCountry(int id)
        {
            return db.Country.Where(c => c.ID == id);
        }

        // GET: api/Countries/Confederation?id={id}
        [ActionName("Confederation")]
        public IQueryable<Country> GetCountryByConfederation(int confederationID)
        {
            return db.Country.Where(c => c.Confederation == confederationID);
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