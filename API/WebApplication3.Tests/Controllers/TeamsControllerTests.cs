using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebApplication3.Models;

namespace WebApplication3.Controllers.Tests
{
    [TestFixture()]
    public class TeamsControllerTests
    {
        private dbapitoEntities Mocked()
        {
            var data = new List<Team>
            {
                new Team { ID = 1, TeamName = "A", TeamCompleteName = "B" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Team>>();
            mockSet.As<IQueryable<Team>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Team>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Team>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Team>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mock = new Mock<dbapitoEntities>();
            mock.Setup(m => m.Team).Returns(mockSet.Object);

            return mock.Object;
        }
        
        [Test()]
        [TestCase("", "")]
        [TestCase("A", "")]
        [TestCase("", "A")]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        public void MustReturnEmptyTeamObjectIfAnyArgumentIsInvalid(string name, string completeName)
        {
            TeamsController teamsController = new TeamsController(Mocked());
            var team = teamsController.GetTeamByName(name, completeName);

            Assert.IsNotNull(team);
            Assert.IsInstanceOf<Team>(team);
            Assert.AreEqual(0, team.ID);
        }

        [Test()]
        [TestCase("A", "B")]
        public void MustReturnTeamIfArgumentsAreValid(string name, string completeName)
        {
            TeamsController teamsController = new TeamsController(Mocked());
            var team = teamsController.GetTeamByName(name, completeName);

            Assert.IsNotNull(team);
            Assert.IsInstanceOf<Team>(team);
            Assert.AreEqual(1, team.ID);
            Assert.AreEqual(name, team.TeamName);
            Assert.AreEqual(completeName, team.TeamCompleteName);
        }

        [Test()]
        [TestCase("B", "C")]
        public void MustCallPostTeamMethodIfTeamDoesNotExist(string name, string completeName)
        {
            var mockedTeamsController = new Mock<TeamsController>(Mocked());
            mockedTeamsController.Setup(c => c.PostTeam(It.IsAny<Team>())).Returns(false); //Must return false to do not enter in a loop, as it is a recursive method
            mockedTeamsController.Object.GetTeamByName(name, completeName);

            mockedTeamsController.Verify(t => t.PostTeam(It.IsAny<Team>()));
        }

        [Test()]
        [TestCase(0)]
        [TestCase(null)]
        [TestCase(1)]
        public void MustReturnTeamIfExistsOrEmptyTeamObjectIfIdIsInvalid(int id)
        {
            TeamsController teamsController = new TeamsController(Mocked());
            var team = teamsController.GetTeamByID(id);

            Assert.IsNotNull(team);
            Assert.IsInstanceOf<Team>(team);
            Assert.AreEqual(id, team.ID);
        }
    }
}