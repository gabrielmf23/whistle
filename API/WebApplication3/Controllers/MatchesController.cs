using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class MatchesController : ApiController
    {
        private dbapitoEntities db;
        public MatchesController(dbapitoEntities dbapito) => db = dbapito;
        public MatchesController() => db = new dbapitoEntities();
        private RefereesController referee = new RefereesController();
        private TeamsController team = new TeamsController();
        public NextMatchesController nextMatchesController = new NextMatchesController();
        private ChampionshipsController championship = new ChampionshipsController();

        // GET: api/Matches
        public IEnumerable<Matches> GetMatches()
        {
            return db.Matches.ToList();
        }

        // POST: api/Matches
        [HttpPost]
        public IHttpActionResult PostMatches([FromBody]JToken jsonbody)
        {
            try
            {
                Matches match = ValidateMatchObject(MountMatchObject(jsonbody));

                //Next match
                if (!match.Result.Equals("N") && !match.Result.Equals("P"))
                {
                    // To ensure that the old "Next match" record is removed, no matter what
                    DeleteOldNextMatchesRecords(match);

                    if (!match.Result.Equals('C'))
                    {
                        //Check if the match isn't already in the database
                        if (!MatchesExists(match.HomeTeam, match.AwayTeam, match.MatchDate))
                        {
                            if (ModelState.IsValid)
                            {
                                try
                                {
                                    db.Matches.Add(match);
                                    db.SaveChanges();
                                    return Ok();
                                }
                                catch (Exception e)
                                {
                                    try
                                    {
                                        db.Matches.Remove(match);
                                    }
                                    catch { }
                                    return InternalServerError(e);
                                }
                            }
                            return BadRequest(ModelState);
                        }
                        return Conflict();
                    }// C - Cancelled match, should just remove the record from NextMatches table
                    return Ok();
                }
                else
                {
                    if (match.Result.Equals("N"))
                    {
                        if (PostNextMatch(match))
                            return Ok();
                        return InternalServerError();
                    }
                    if (UpdateNextMatch(match))
                        return Ok();
                    return InternalServerError();
                }
            }
            catch
            {
                return InternalServerError();
            }
        }

        public virtual Matches ValidateMatchObject(Matches match)
        {
            if (match == null)
                return match;

            //Validate the championship
            if (match.Championship.Equals(0))
                return null;

            //validate critical data
            if (match.Referee.Equals(0) || match.HomeTeam.Equals(0) || match.AwayTeam.Equals(0))
                return null;

            if (match.HomeTeam.Equals(match.AwayTeam))
                return null;

            //validate possible "result" values
            IList<string> results = new List<string> { "W", "L", "D", "N", "P", "C" };
            bool invalid = true;
            foreach (var result in results)
            {
                if (match.Result.Equals(result))
                {
                    invalid = false;
                    break;
                }
            }
            if (invalid)
                return null;

            return match;
        }

        public virtual Matches MountMatchObject(JToken jToken)
        {
            try
            {
                JObject jObject = JObject.Parse(jToken.Value<string>());

                var match = new Matches
                {
                    Referee = referee.GetRefereeByName(jObject["referee"].ToString()).ID,
                    Result = jObject["match_result"].ToString(),
                    MatchDate = Convert.ToDateTime(jObject["match_date"].ToString()),
                    Championship = championship.GetChampionshipById(Convert.ToInt32(jObject["championship"].ToString())).ID,

                    //Home team data
                    HomeTeam = team.GetTeamByName(jObject["home_team_name"].ToString(), jObject["home_team_complete_name"].ToString()).ID,

                    Home_Goals_Period_1 = Convert.ToInt32(jObject["home_team_goals_1st_period"].ToString()),
                    Home_Goals_Period_2 = Convert.ToInt32(jObject["home_team_goals_2nd_period"].ToString()),
                    Home_Goals_Total = Convert.ToInt32(jObject["home_team_goals_total"].ToString()),

                    Home_Yellow_Card1_Period_1 = Convert.ToInt32(jObject["home_team_1st_yellow_card_1st_period"].ToString()),
                    Home_Yellow_Card1_Period_2 = Convert.ToInt32(jObject["home_team_1st_yellow_card_2nd_period"].ToString()),
                    Home_Yellow_Card1_Total = Convert.ToInt32(jObject["home_team_1st_yellow_card_total"].ToString()),

                    Home_Yellow_Card2_Period_1 = Convert.ToInt32(jObject["home_team_2nd_yellow_card_1st_period"].ToString()),
                    Home_Yellow_Card2_Period_2 = Convert.ToInt32(jObject["home_team_2nd_yellow_card_2nd_period"].ToString()),
                    Home_Yellow_Card2_Total = Convert.ToInt32(jObject["home_team_2nd_yellow_card_total"].ToString()),

                    Home_Red_Card_Period_1 = Convert.ToInt32(jObject["home_team_red_card_1st_period"].ToString()),
                    Home_Red_Card_Period_2 = Convert.ToInt32(jObject["home_team_red_card_2nd_period"].ToString()),
                    Home_Red_Card_Total = Convert.ToInt32(jObject["home_team_red_card_total"].ToString()),

                    //Away team data
                    AwayTeam = team.GetTeamByName(jObject["away_team_name"].ToString(), jObject["away_team_complete_name"].ToString()).ID,

                    Away_Goals_Period_1 = Convert.ToInt32(jObject["away_team_goals_1st_period"].ToString()),
                    Away_Goals_Period_2 = Convert.ToInt32(jObject["away_team_goals_2nd_period"].ToString()),
                    Away_Goals_Total = Convert.ToInt32(jObject["away_team_goals_total"].ToString()),

                    Away_Yellow_Card1_Period_1 = Convert.ToInt32(jObject["away_team_1st_yellow_card_1st_period"].ToString()),
                    Away_Yellow_Card1_Period_2 = Convert.ToInt32(jObject["away_team_1st_yellow_card_2nd_period"].ToString()),
                    Away_Yellow_Card1_Total = Convert.ToInt32(jObject["away_team_1st_yellow_card_total"].ToString()),

                    Away_Yellow_Card2_Period_1 = Convert.ToInt32(jObject["away_team_2nd_yellow_card_1st_period"].ToString()),
                    Away_Yellow_Card2_Period_2 = Convert.ToInt32(jObject["away_team_2nd_yellow_card_2nd_period"].ToString()),
                    Away_Yellow_Card2_Total = Convert.ToInt32(jObject["away_team_2nd_yellow_card_total"].ToString()),

                    Away_Red_Card_Period_1 = Convert.ToInt32(jObject["away_team_red_card_1st_period"].ToString()),
                    Away_Red_Card_Period_2 = Convert.ToInt32(jObject["away_team_red_card_2nd_period"].ToString()),
                    Away_Red_Card_Total = Convert.ToInt32(jObject["away_team_red_card_total"].ToString())
                };

                return match;
            }
            catch
            {
                return null;
            }
        }

        private bool MatchesExists(int home, int away, DateTime matchDate)
        {
            return db.Matches.Count(m => m.HomeTeam == home && m.AwayTeam == away && m.MatchDate == matchDate) > 0;
        }

        public virtual void DeleteOldNextMatchesRecords(Matches match)
        {
            nextMatchesController.DeleteOldRecords(match);
        }

        public virtual bool PostNextMatch(Matches match)
        {
            return nextMatchesController.PostNextMatch(match);
        }

        public virtual bool UpdateNextMatch(Matches match)
        {
            return nextMatchesController.UpdateNextMatch(match);
        }

        #region Auto generated methods

        /*
        // PUT: api/Matches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatches(int id, Matches matches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matches.Referee)
            {
                return BadRequest();
            }

            db.Entry(matches).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchesExists(id))
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

        // DELETE: api/Matches/5
        [ResponseType(typeof(Matches))]
        public async Task<IHttpActionResult> DeleteMatches(int id)
        {
            Matches matches = await db.Matches.FindAsync(id);
            if (matches == null)
            {
                return NotFound();
            }

            db.Matches.Remove(matches);
            await db.SaveChangesAsync();

            return Ok(matches);
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