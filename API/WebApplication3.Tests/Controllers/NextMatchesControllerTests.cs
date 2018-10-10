using WebApplication3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApplication3.Models;
using Moq;
using System.Data.Entity;

namespace WebApplication3.Controllers.Tests
{
    [TestFixture()]
    public class NextMatchesControllerTests
    {
        private dbapitoEntities Mocked()
        {
            var data = new List<NextMatch>
            {
                new NextMatch { Referee = 1, SelectedTeam = 1, AgainstTeam = 2, Championship = 1, MatchDate = (DateTime.Today).AddDays(7), FieldControl = "H" },
                new NextMatch { Referee = 1, SelectedTeam = 2, AgainstTeam = 1, Championship = 1, MatchDate = (DateTime.Today).AddDays(7), FieldControl = "V" }

            }.AsQueryable();

            var mockSet = new Mock<DbSet<NextMatch>>();
            mockSet.As<IQueryable<NextMatch>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<NextMatch>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<NextMatch>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<NextMatch>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mock = new Mock<dbapitoEntities>();
            mock.Setup(m => m.NextMatch).Returns(mockSet.Object);

            return mock.Object;
        }
        
        [Test()]
        public void ShouldReturnListWithTwoNextMatchObjects()
        {
            NextMatchesController nextMatchesController = new NextMatchesController(Mocked());
            var nextMatches = nextMatchesController.CreateNextMatches(new Mock<Matches>().Object);

            Assert.AreEqual(2, nextMatches.Count());
            Assert.IsInstanceOf<NextMatch>(nextMatches[0]);
            Assert.IsInstanceOf<NextMatch>(nextMatches[1]);
        }
    }
}