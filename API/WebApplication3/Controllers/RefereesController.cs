using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class RefereesController : ApiController
    {
        private dbapitoEntities db;

        public RefereesController(dbapitoEntities dbapito) => db = dbapito;

        public RefereesController() => db = new dbapitoEntities();

        // GET: api/Referees
        public IQueryable<Referee> GetReferee()
        {
            return db.Referee;
        }

        public Referee GetRefereeByID(int id)
        {
            Referee referee = db.Referee.ToList<Referee>().Find(r => r.ID == id);
            if (referee == null)
                referee = new Referee { ID = 0 };
            return referee;
        }

        public Referee GetRefereeByName(string name)
        {
            if (name == null || name.Length.Equals(0))
                return new Referee { ID = 0 };

            Referee referee = db.Referee.ToList<Referee>().Find(r => r.RefereeName == name);
            if (referee == null)
                referee = PostReferee(new Referee { RefereeName = name }) ? GetRefereeByName(name) : referee = new Referee { ID = 0 };
            return referee;            
        }

        public virtual bool PostReferee(Referee referee)
        {
           try
            {
                db.Referee.Add(referee);
                db.SaveChanges();
                return true;
            }
            catch
            {
                try
                {
                    db.Referee.Remove(referee);
                }
                catch { }
                return false;
            }
        }
                
        #region Auto generated methods
        /*
        // PUT: api/Referees/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReferee(int id, Referee referee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != referee.ID)
            {
                return BadRequest();
            }

            db.Entry(referee).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefereeExists(id))
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

        // DELETE: api/Referees/5
        [ResponseType(typeof(Referee))]
        public async Task<IHttpActionResult> DeleteReferee(int id)
        {
            Referee referee = await db.Referee.FindAsync(id);
            if (referee == null)
            {
                return NotFound();
            }

            db.Referee.Remove(referee);
            await db.SaveChangesAsync();

            return Ok(referee);
        }

        private bool RefereeExists(int id)
        {
            return db.Referee.Count(e => e.ID == id) > 0;
        }
        */
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