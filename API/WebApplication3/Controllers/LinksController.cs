using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class LinksController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();

        // GET: api/Links
        public IEnumerable<Links> GetLinks()
        {
            return db.Links.ToList();
        }

        #region Auto generated methods
        /*
        // GET: api/Links/5
        [ResponseType(typeof(Links))]
        public async Task<IHttpActionResult> GetLinks(int id)
        {
            Links links = await db.Links.FindAsync(id);
            if (links == null)
            {
                return NotFound();
            }

            return Ok(links);
        }

        // PUT: api/Links/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLinks(int id, Links links)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != links.ChampionshipID)
            {
                return BadRequest();
            }

            db.Entry(links).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LinksExists(id))
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

        // POST: api/Links
        [ResponseType(typeof(Links))]
        public async Task<IHttpActionResult> PostLinks(Links links)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Links.Add(links);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LinksExists(links.ChampionshipID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = links.ChampionshipID }, links);
        }

        // DELETE: api/Links/5
        [ResponseType(typeof(Links))]
        public async Task<IHttpActionResult> DeleteLinks(int id)
        {
            Links links = await db.Links.FindAsync(id);
            if (links == null)
            {
                return NotFound();
            }

            db.Links.Remove(links);
            await db.SaveChangesAsync();

            return Ok(links);
        }

        private bool LinksExists(int id)
        {
            return db.Links.Count(e => e.ChampionshipID == id) > 0;
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