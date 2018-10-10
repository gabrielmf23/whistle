using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WebApplication3.Models;
using Moq;
using System.Data.Entity;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace WebApplication3.Controllers.Tests
{
    [TestFixture()]
    public class MatchesControllerTests
    {
        private dbapitoEntities Mocked()
        {
            var data = new List<Matches>
            {
                new Matches { Referee = 1, HomeTeam = 1, AwayTeam = 2, Championship = 1, MatchDate = DateTime.Today.AddDays(-1), YC_Home = 0, RC_Home = 0, YC_Away = 0, RC_Away = 0, Result = "D" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Matches>>();
            mockSet.As<IQueryable<Matches>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Matches>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Matches>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Matches>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mock = new Mock<dbapitoEntities>();
            mock.Setup(m => m.Matches).Returns(mockSet.Object);

            return mock.Object;
        }

        private JToken Parser(string json)
        {
            try
            {
                return JToken.Parse(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Matches MockedMatch(string matchResult)
        {
            Matches match = Mock.Of<Matches>(m => m.Championship == 1 && m.Referee == 1 && m.HomeTeam == 1 && m.AwayTeam == 2 && m.Result == matchResult);
            return match;
        }

        [Test()]
        [TestCase("{'py/object': 'Match.Match', '': 'Gr\u00eamio FB Porto Alegrense', 'aw_name': 'Gr\u00eamio', 'aw_r_card': 1, 'aw_y_card': 1, 'championship': 1, 'ht_complete_name': 'Cruzeiro EC', 'ht_name': 'Cruzeiro', 'ht_r_card': 0, 'ht_y_card': 3, 'match_date': '2018-4-14', 'match_result': 'L', 'referee': 'Rodolpho Toski Marques'}")]
        [TestCase(null)]
        public void MustReturnNullIfJSONIsInvalid(object jtoken)
        {
            JToken token = Parser((String)jtoken);
            MatchesController matchesController = new MatchesController(Mocked());
            var match = matchesController.MountMatchObject(token);

            NUnit.Framework.Assert.AreEqual(null, match);
        }

        [Test()]
        [TestCase("C")]
        [TestCase("R")]
        [TestCase("H")]
        [TestCase("A")]
        public void MountAndValidateMatchObjectMethodMustReturnNullIfChampionshipOrRefereeOrTeamsAreInvallid(string option)
        {
            MatchesController matchesController = new MatchesController(Mocked());
            Matches match;
            switch (option)
            {
                case "C":
                    match = matchesController.ValidateMatchObject(new Matches { });
                    break;
                case "R":
                    match = matchesController.ValidateMatchObject(new Matches { Championship = 1, HomeTeam = 1, AwayTeam = 2 });
                    break;
                case "H":
                    match = matchesController.ValidateMatchObject(new Matches { Championship = 1, Referee = 1, AwayTeam = 2 });
                    break;
                case "A":
                    match = matchesController.ValidateMatchObject(new Matches { Championship = 1, Referee = 1, HomeTeam = 2 });
                    break;
                default:
                    match = new Matches();
                    break;
            }
            
            NUnit.Framework.Assert.AreEqual(null, match);
        }

        [Test()]
        public void MustReturnNullIfTeamsAreEqual()
        {
            var matchesController = new MatchesController(Mocked());
            var match = matchesController.ValidateMatchObject(new Matches { Championship = 1, Referee = 1, HomeTeam = 1, AwayTeam = 1 });

            NUnit.Framework.Assert.AreEqual(null, match);
        }

        [Test()]
        [TestCase("W")]
        [TestCase("L")]
        [TestCase("D")]
        [TestCase("N")]
        [TestCase("P")]
        [TestCase("C")]
        [TestCase("X")]
        public void MustReturnMatchObjectIfResultIsValidAndNullIfInvalid(string result)
        {
            var matchesController = new MatchesController(Mocked());
            Matches match = matchesController.ValidateMatchObject(new Matches { Championship = 1, Referee = 1, HomeTeam = 1, AwayTeam = 2, Result = result });

            if (result != "X")
                NUnit.Framework.Assert.IsInstanceOf<Matches>(match);
            else
                NUnit.Framework.Assert.AreEqual(null, match);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void MustThrowExceptionIfMatchIsNull()
        {
            var matchesController = new MatchesController(Mocked());
            matchesController.PostMatches(null);
        }

        [Test()]
        public void MustCallPostNextMatchMethodForANextMatch()
        {
            var matchesController = new Mock<MatchesController>(Mocked());
            matchesController.Setup(m => m.MountMatchObject(It.IsAny<JToken>())).Returns(It.IsAny<Matches>());
            matchesController.Setup(m => m.ValidateMatchObject(It.IsAny<Matches>())).Returns<Matches>(m => MockedMatch("N"));//Next Match

            matchesController.Object.PostMatches(It.IsAny<JToken>());

            matchesController.Verify(m => m.PostNextMatch(It.IsAny<Matches>()));
        }

        [Test()]
        public void MustCallUpdateNextMatchMethodForAPostponedMatch()
        {
            var matchesController = new Mock<MatchesController>(Mocked());
            matchesController.Setup(m => m.MountMatchObject(It.IsAny<JToken>())).Returns(It.IsAny<Matches>());
            matchesController.Setup(m => m.ValidateMatchObject(It.IsAny<Matches>())).Returns(MockedMatch("P"));//Postponed Match

            matchesController.Object.PostMatches(It.IsAny<JToken>());

            matchesController.Verify(m => m.UpdateNextMatch(It.IsAny<Matches>()));
        }

        [Test()]
        [TestCase("W")]
        [TestCase("L")]
        [TestCase("D")]
        [TestCase("C")]
        public void MustCallDeleteOldNextMatchMethodForACancelledOrNewMatch(string option)
        {
            var matchesController = new Mock<MatchesController>(Mocked());
            matchesController.Setup(m => m.MountMatchObject(It.IsAny<JToken>())).Returns(It.IsAny<Matches>());
            matchesController.Setup(m => m.ValidateMatchObject(It.IsAny<Matches>())).Returns(MockedMatch(option));
            matchesController.Setup(m => m.DeleteOldNextMatchesRecords(It.IsAny<Matches>()));
            

            matchesController.Object.PostMatches(It.IsAny<JToken>());

            matchesController.Verify(m => m.DeleteOldNextMatchesRecords(It.IsAny<Matches>()));
        }
    }
}