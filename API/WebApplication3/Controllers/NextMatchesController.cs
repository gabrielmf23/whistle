using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class NextMatchesController : ApiController
    {
        private dbapitoEntities db;

        public NextMatchesController(dbapitoEntities dbapito) => db = dbapito;

        public NextMatchesController() => db = new dbapitoEntities();

        // GET: api/NextMatches
        public IQueryable<NextMatch> GetNextMatches()
        {
            return db.NextMatch;
        }

        public virtual List<NextMatch> CreateNextMatches(Matches match)
        {
            List<NextMatch> nextMatches = new List<NextMatch>();

            for (int i = 0; i < 2; i++)
            {
                NextMatch nm = new NextMatch
                {
                    Referee = match.Referee,
                    Championship = match.Championship,
                    MatchDate = match.MatchDate
                };
                if (i.Equals(0))
                {
                    nm.SelectedTeam = match.HomeTeam;
                    nm.AgainstTeam = match.AwayTeam;
                    nm.FieldControl = "H";
                    nextMatches.Add(nm);
                    continue;
                }
                nm.SelectedTeam = match.AwayTeam;
                nm.AgainstTeam = match.HomeTeam;
                nm.FieldControl = "V";
                nextMatches.Add(nm);
            }
            return nextMatches;
        }

        public virtual bool PostNextMatch(Matches match)
        {
            List<NextMatch> nextMatches = CreateNextMatches(match);

            int ignored = 0;

            foreach (var nextMatch in nextMatches)
            {
                //Check if team already has a next match scheduled
                if (CheckNextMatch(nextMatch))
                {
                    ignored += 1;
                    continue;
                }

                if (ModelState.IsValid)
                    db.NextMatch.Add(nextMatch);
            }

            //Check if there's something to save
            if (!ignored.Equals(2))
            {
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch { }
            }
            return false;
        }

        private bool CheckNextMatch(NextMatch nextMatch)
        {
            var oldRecord = db.NextMatch.ToList<NextMatch>().Find(nm => nm.SelectedTeam == nextMatch.SelectedTeam && nm.Championship == nextMatch.Championship);

            if (oldRecord == null)
                return false;
            else
            {
                if (!(oldRecord.MatchDate < DateTime.Today) && oldRecord.MatchDate < nextMatch.MatchDate)
                {
                    //Already has a most recent "next match" scheduled
                    //Action: ignore
                    return true;
                }
                if (oldRecord.MatchDate == nextMatch.MatchDate && oldRecord.AgainstTeam == nextMatch.AgainstTeam && oldRecord.FieldControl == nextMatch.FieldControl)
                {
                    //Is the same match
                    //Acion: ignore
                    return true;
                }
                else
                {
                    //Has a "next match" but is outdated or isn't the closest one
                    //Action: Delete the old record
                    DeleteNextMatch(oldRecord);
                    return false;
                }                
            }
        }

        public void DeleteOldRecords(Matches match)
        {
            List<NextMatch> nextMatches = CreateNextMatches(match);

            foreach (var nextMatch in nextMatches)
            {
                DeleteNextMatch(nextMatch);
            }
        }

        private void DeleteNextMatch(NextMatch nextMatch)
        {
            if (nextMatch != null)
            {
                if (NextMatchExists(nextMatch))
                {
                    try
                    {
                        db.NextMatch.Remove(nextMatch);
                        db.SaveChanges();
                    }
                    catch { }
                }
            }
        }

        public bool UpdateNextMatch(Matches match)
        {
            int ignored = 0;
            foreach (var nextMatch in CreateNextMatches(match))
            {
                if (NextMatchExists(nextMatch))
                {
                    var i = db.NextMatch.ToList<NextMatch>().FindIndex(nm => nm.SelectedTeam == nextMatch.SelectedTeam && nm.AgainstTeam == nextMatch.AgainstTeam && nm.Championship == nextMatch.Championship);
                    db.NextMatch.ToList<NextMatch>()[i].MatchDate = nextMatch.MatchDate;
                }
                else
                    ignored += 1;
            }
            if (ignored.Equals(2))
            {
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch { }
            }
            return false;
        }

        private bool NextMatchExists(NextMatch nextMatch)
        {
            return db.NextMatch.Count(nm => nm.SelectedTeam == nextMatch.SelectedTeam && nm.AgainstTeam == nextMatch.AgainstTeam && nm.Championship == nextMatch.Championship) > 0;
        }

        #region Auto generated methods

        /*
         * // GET: api/NextMatches/5
        [ResponseType(typeof(NextMatch))]
        public IHttpActionResult GetNextMatch(int id)
        {
            NextMatch nextMatch = db.NextMatch.Find(id);
            if (nextMatch == null)
            {
                return NotFound();
            }

            return Ok(nextMatch);
        }

        // PUT: api/NextMatches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNextMatch(int id, NextMatch nextMatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nextMatch.Referee)
            {
                return BadRequest();
            }

            db.Entry(nextMatch).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NextMatchExists(id))
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