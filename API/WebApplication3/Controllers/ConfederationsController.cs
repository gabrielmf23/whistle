using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ConfederationsController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();

        // GET: api/Confederations
        public IEnumerable<Confederation> GetConfederation()
        {
            return db.Confederation.ToList();
        }

        // GET: api/Confederations/id
        public Confederation GetConfederationById(int id)
        {
            Confederation confederation = new Confederation();

            try
            {
                confederation = db.Confederation.ToList<Confederation>().Find(c => c.ID == id);
            }
            catch
            {
                confederation.ID = 0;
            }

            return confederation;
        }        

        #region Auto generated methods
        /*
        // PUT: api/Confederations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutConfederation(int id, Confederation confederation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != confederation.ID)
            {
                return BadRequest();
            }

            db.Entry(confederation).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfederationExists(id))
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

        // POST: api/Confederations
        [ResponseType(typeof(Confederation))]
        public async Task<IHttpActionResult> PostConfederation(Confederation confederation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Confederation.Add(confederation);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = confederation.ID }, confederation);
        }

        // DELETE: api/Confederations/5
        [ResponseType(typeof(Confederation))]
        public async Task<IHttpActionResult> DeleteConfederation(int id)
        {
            Confederation confederation = await db.Confederation.FindAsync(id);
            if (confederation == null)
            {
                return NotFound();
            }

            db.Confederation.Remove(confederation);
            await db.SaveChangesAsync();

            return Ok(confederation);
        }

        private bool ConfederationExists(int id)
        {
            return db.Confederation.Count(e => e.ID == id) > 0;
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