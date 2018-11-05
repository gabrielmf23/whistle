using System;
using System.Linq;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class StatisticsController : ApiController
    {
        private dbapitoEntities db = new dbapitoEntities();

        public partial class MatchStatistics
        {
            public int VictoriesTeamA;
            public int VictoriesTeamB;
            public int Draws;
        }

        // GET: api/NextMatches/Confederation/{id}
        [Route("api/Statistics/id={id}/home={home}/away={away}")]
        public MatchStatistics GetGeneralResultsByChampionship(int id, int home, int away)
        {
            try
            {
                MatchStatistics matchStatistics = new MatchStatistics();

                try
                {
                    matchStatistics.VictoriesTeamA = (from m in db.Matches
                                                      where m.Championship == id &&
                                                      ((m.HomeTeam == home && m.AwayTeam == away && m.Result == "W") || (m.HomeTeam == away && m.AwayTeam == home && m.Result == "L"))
                                                      select new { m.Referee }).Count();
                }
                catch
                {
                    matchStatistics.VictoriesTeamA = 0;
                }


                try
                {
                    matchStatistics.VictoriesTeamB = (from m in db.Matches
                                                      where m.Championship == id &&
                                                      ((m.HomeTeam == home && m.AwayTeam == away && m.Result == "L") || (m.HomeTeam == away && m.AwayTeam == home && m.Result == "W"))
                                                      select new { m.Referee }).Count();
                }
                catch
                {
                    matchStatistics.VictoriesTeamB = 0;
                }

                try
                {
                    matchStatistics.Draws = (from m in db.Matches
                                             where m.Championship == id &&
                                             m.Result == "D" &&
                                             ((m.HomeTeam == home && m.AwayTeam == away) || (m.HomeTeam == away && m.AwayTeam == home))
                                             select new { m.Referee }).Count();
                }
                catch
                {
                    matchStatistics.Draws = 0;
                }

                return matchStatistics;
            }
            catch
            {
                return null;
            }
        }
    }
}
