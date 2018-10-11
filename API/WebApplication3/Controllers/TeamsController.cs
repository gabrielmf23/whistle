using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class TeamsController : ApiController
    {
        private dbapitoEntities db;

        public TeamsController(dbapitoEntities dbapito) => db = dbapito;

        public TeamsController() => db = new dbapitoEntities();

        // GET: api/Teams
        public IEnumerable<Team> GetTeam()
        {
            return db.Team.ToList();
        }

        public Team GetTeamByID(int id)
        {
            Team team = db.Team.ToList<Team>().Find(t => t.ID == id);
            if (team == null)
                return new Team { ID = 0 };
            return team;
        }

        public Team GetTeamByName(string name, string completeName)
        {
            if (name == null || completeName == null || name.Length.Equals(0) || completeName.Length.Equals(0))
                return new Team { ID = 0 };

            Team team = db.Team.ToList<Team>().Find(t => t.TeamName == name && t.TeamCompleteName == completeName);
            if (team == null)
                team = PostTeam(new Team { TeamName = name, TeamCompleteName = completeName }) ? GetTeamByName(name, completeName) : new Team { ID = 0 };
            return team;
        }

        public virtual bool PostTeam(Team team)
        {
            try
            {
                db.Team.Add(team);
                db.SaveChanges();
                return true;
            }
            catch
            {
                try
                {
                    db.Team.Remove(team);
                }
                catch { }
            }
            return false;
        }

        #region Auto generated methods
        /*
        // PUT: api/Teams/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTeam(int id, Team team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != team.ID)
            {
                return BadRequest();
            }

            db.Entry(team).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // DELETE: api/Teams/5
        [ResponseType(typeof(Team))]
        public async Task<IHttpActionResult> DeleteTeam(int id)
        {
            Team team = await db.Team.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            db.Team.Remove(team);
            await db.SaveChangesAsync();

            return Ok(team);
        }

        private bool TeamExists(int id)
        {
            return db.Team.Count(e => e.ID == id) > 0;
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