using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ChampionshipsController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();
        private CountriesController countriesController = new CountriesController();

        // GET: api/Championships
        public IEnumerable<Championship> GetChampionships()
        {
            return db.Championship.ToList();
        }

        // GET: api/Championships/{id}
        public Championship GetChampionshipById(int id)
        {
            Championship champ = new Championship();

            try
            {
                champ = db.Championship.ToList().Find(c => c.ID == id);
            }
            catch
            {
                champ.ID = 0;
            }

            return champ;
        }

        [Route("api/Championships/Country/{id}")]
        public IEnumerable<Championship> GetChampionshipsByCountry(int id)
        {
            //When a country is selected, it should show all championships related to that country, but also the championships related to the confederation which the country is part of
            //i.e.: Selected Country: England
            //Should return any english championships, and european championships, as Champions/Europa League

            try
            {
               return db.Championship.ToList().FindAll(c => c.Country == id);
            }
            catch
            {
                return new List<Championship>();
            }
        }

        // GET: api/Championships/Confederation/{id}
        [Route("api/Championships/Confederation/{id}")]
        public IEnumerable<Championship> GetChampionshipsByConfederation(int id)
        {
            try
            {
                IList<Championship> championships = new List<Championship>();
                var countries = countriesController.GetCountriesByConfederation(id);

                foreach (Country country in countries)
                {
                    foreach (Championship championship in db.Championship.ToList())
                    {
                        if (championship.Country == country.ID)
                        {
                            championships.Add(championship);
                        }
                    }
                }
                return championships;
            }
            catch
            {
                return new List<Championship>();
            }
        }

        #region Auto generated methods
        /*
        // PUT: api/Championships/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutChampionship(int id, Championship championship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != championship.ID)
            {
                return BadRequest();
            }

            db.Entry(championship).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChampionshipExists(id))
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

        // POST: api/Championships
        [ResponseType(typeof(Championship))]
        public async Task<IHttpActionResult> PostChampionship(Championship championship)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Championship.Add(championship);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = championship.ID }, championship);
        }

        // DELETE: api/Championships/5
        [ResponseType(typeof(Championship))]
        public async Task<IHttpActionResult> DeleteChampionship(int id)
        {
            Championship championship = await db.Championship.FindAsync(id);
            if (championship == null)
            {
                return NotFound();
            }

            db.Championship.Remove(championship);
            await db.SaveChangesAsync();

            return Ok(championship);
        }

        private bool ChampionshipExists(int id)
        {
            return db.Championship.Count(e => e.ID == id) > 0;
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