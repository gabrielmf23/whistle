using WebApplication3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Data.Entity;
using WebApplication3.Models;

namespace WebApplication3.Controllers.Tests
{
    [TestFixture()]
    public class RefereesControllerTests
    {
        private dbapitoEntities Mocked()
        {
            var data = new List<Referee>
            {
                new Referee { ID = 1, RefereeName = "A" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Referee>>();
            mockSet.As<IQueryable<Referee>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Referee>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Referee>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Referee>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mock = new Mock<dbapitoEntities>();
            mock.Setup(m => m.Referee).Returns(mockSet.Object);

            return mock.Object;
        }

        [Test()]
        [TestCase("")]
        [TestCase(null)]
        public void MustReturnEmptyRefereeObjectIfAnyArgumentIsInvalid(string name)
        {
            RefereesController refereesController = new RefereesController(Mocked());
            var referee = refereesController.GetRefereeByName(name);

            Assert.IsNotNull(referee);
            Assert.IsInstanceOf<Referee>(referee);
            Assert.AreEqual(0, referee.ID);
        }

        [Test()]
        [TestCase("A")]
        public void MustReturnRefereeIfArgumentIsValid(string name)
        {
            RefereesController refereesController = new RefereesController(Mocked());
            var referee = refereesController.GetRefereeByName(name);

            Assert.IsNotNull(referee);
            Assert.IsInstanceOf<Referee>(referee);
            Assert.AreEqual(1, referee.ID);
            Assert.AreEqual(name, referee.RefereeName);
        }

        [Test()]
        [TestCase("B")]
        public void MustCallPostRefereeMethodIfRefereeDoesNotExist(string name)
        {
            var refereesController = new Mock<RefereesController>(Mocked());
            refereesController.Setup(c => c.PostReferee(It.IsAny<Referee>())).Returns(false); //Must return false to do not enter in a loop, as it is a recursive method
            refereesController.Object.GetRefereeByName(name);

            refereesController.Verify(t => t.PostReferee(It.IsAny<Referee>()));
        }

        [Test()]
        [TestCase(0)]
        [TestCase(null)]
        [TestCase(1)]
        public void MustReturnRefereeIfExistsOrEmptyRefereeObjectIfIdIsInvalid(int id)
        {
            RefereesController refereesController = new RefereesController(Mocked());
            var referee = refereesController.GetRefereeByID(id);

            Assert.IsNotNull(referee);
            Assert.IsInstanceOf<Referee>(referee);
            Assert.AreEqual(id, referee.ID);
        }
    }
}