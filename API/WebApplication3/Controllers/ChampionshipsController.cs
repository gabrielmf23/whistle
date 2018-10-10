using System.Data;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ChampionshipsController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();

        // GET: api/Championships
        public IQueryable<Championship> GetChampionship()
        {
            return db.Championship;
        }

        // GET: api/Championships/{id}
        public Championship GetChampionshipById(int id)
        {
            Championship champ = new Championship();

            try
            {
                champ = db.Championship.ToList<Championship>().Find(c => c.ID == id);
            }
            catch
            {
                champ.ID = 0;
            }

            return champ;
        }

        // GET: api/Championships/Country?id={id}
        [ActionName("Country")]
        public IQueryable<Championship> GetChampionshipByCountry(int countryID)
        {
            //When a country is selected, it should show all championships related to that country, but also the championships related to the confederation which the country is part of
            //i.e.: Selected Country: England
            //Should return any english championships, and european championships, as Champions/Europa League
            return db.Championship.Where(c => c.Country == countryID && c.Country == c.Country1.Confederation);
        }

        // GET: api/Championships/Confederation?id={id}
        [ActionName("Confederation")]
        public IQueryable<Championship> GetChampionshipByConfederation(int confederationID)
        {
            return db.Championship.Where(c => c.Country1.Confederation == confederationID);
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