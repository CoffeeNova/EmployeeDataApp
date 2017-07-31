using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using EmployeeDataApp.Helpers;
using EmployeeDataApp.Models;
using EmployeeDataApp.Tests;
using Moq;
using KellermanSoftware.CompareNetObjects;

namespace EmployeeDataApp.Controllers.Tests
{

    [TestFixture]
    public class EmployeeControllerTests
    {
        [OneTimeSetUp]
        public static void SetUpFixture()
        {
            _config = new ComparisonConfig
            {
                CompareChildren = true,
                CompareFields = false,
                CompareReadOnly = true,
                ComparePrivateFields = false,
                ComparePrivateProperties = false,
                CompareProperties = true,
                MaxDifferences = 50
            };
        }

        [SetUp]
        public static void SetUp()
        {
            _testProfModel = new ProfessionModel
            {
                EmployeeModelId = 1,
                ProfessionName = "profone"
            };
            _testEmployeeModel = new EmployeeModel
            {
                Age = 20,
                FirstName = "firstName",
                LastName = "firstSurname",
                Professions = new List<ProfessionModel> { _testProfModel }
            };
            _testEmployeeModel2 = new EmployeeModel
            {
                Age = 30,
                FirstName = "secondName",
                LastName = "secondSurname"
            };
            _testEmployeeDbSet = new TestDbSet<EmployeeModel> { _testEmployeeModel };
            _testProfessionDbSet = new TestDbSet<ProfessionModel> { _testProfModel };
            switch (TestContext.CurrentContext.Test.MethodName)
            {
                case nameof(TestGetAdd_InputDataSet_OkResultWithEmployeeModel):
                    _testEmployeeModel.Professions = new List<ProfessionModel>();
                    break;
                case nameof(TestGetUpdate_Id_Not_Found):
                    _testEmployeeModel.Id = 1;
                    break;
                case nameof(TestGetRemove):
                    _testEmployeeModel.Id = 1;
                    _testEmployeeModel2.Id = 2;
                    _testEmployeeDbSet.Add(_testEmployeeModel2);
                    break;
            }
        }

        [TearDown]
        public static void CleanUp()
        {
            switch (TestContext.CurrentContext.Test.MethodName)
            {
                case nameof(TestGetAdd_InputDataSet_OkResultWithEmployeeModel):
                    _testEmployeeModel.Professions.Add(_testProfModel);
                    break;
                case nameof(TestGetUpdate_Id_Not_Found):
                    _testEmployeeModel.Id = 0;
                    break;
                case nameof(TestGetRemove):
                    _testEmployeeModel.Id = 0;
                    _testEmployeeDbSet.Remove(_testEmployeeModel2);
                    break;
            }
        }

        [Test]
        public void TestGetAllUsers()
        {
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Employees, _testEmployeeDbSet);

            var controller = new EmployeeController(mock.Object);

            var actionResult = controller.All();
            var contentResult = actionResult as OkNegotiatedContentResult<IQueryable<EmployeeModel>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_testEmployeeDbSet, contentResult.Content);
        }

        [Test]
        public void TestProfessions()
        {
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Professions, _testProfessionDbSet);

            var controller = new EmployeeController(mock.Object);
            var actionResult = controller.Professions();

            var contentResult = actionResult as OkNegotiatedContentResult<DbSet<ProfessionModel>>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_testProfessionDbSet, contentResult.Content);
        }

        [Test]
        public void TestGetAdd_Test_Methods_Invocation()
        {
            var helperMock = new Mock<EmployeeControllerHelper>();
            helperMock.Setup(x => x.FindEmployees(It.IsAny<EmployeeModel>())).
                Returns(() =>null);
            var mock = new Mock<IEmployeeDbContext>();
            mock.Setup(x => x.Employees.Add(It.IsAny<EmployeeModel>()))
                .Returns(new EmployeeModel());
            mock.Setup(x => x.SaveChanges())
                .Returns(0);

            var controller = new EmployeeController(mock.Object, helperMock.Object);
            var actionResult = controller.Add("testf", "testl", "30", "female");

            mock.Verify(x => x.Employees.Add(It.IsAny<EmployeeModel>()));
            mock.Verify(x => x.SaveChanges());
            helperMock.Verify(x => x.FindEmployees(It.IsAny<EmployeeModel>()));
        }

        [Test]
        public void TestGetAdd_InputDataSet_OkResultWithEmployeeModel()
        {
            var helperMock = new Mock<EmployeeControllerHelper>();
            helperMock.Setup(x => x.FindEmployees(It.IsAny<EmployeeModel>())).
                Returns(() => null);
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Employees, _testEmployeeDbSet);

            var controller = new EmployeeController(mock.Object, helperMock.Object);
            var expected = new OkNegotiatedContentResult<EmployeeModel>(_testEmployeeModel, controller);

            var actionResult = controller.Add(_testEmployeeModel.FirstName, _testEmployeeModel.LastName,
                                            _testEmployeeModel.Age.ToString(), _testEmployeeModel.Gender);
            var actual = actionResult as OkNegotiatedContentResult<EmployeeModel>;

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Content);

            var compareLogic = new CompareLogic(_config);
            var comparationResult = compareLogic.Compare(expected.Content, actual.Content);

            Assert.IsTrue(comparationResult.AreEqual, comparationResult.DifferencesString);
        }

        [Test]
        public void TestPostAdd_Test_Methods_Invocation()
        {
            var helperMock = new Mock<EmployeeControllerHelper>();
            helperMock.Setup(x => x.FindEmployees(It.IsAny<EmployeeModel>())).
                Returns(() => null);
            var mock = new Mock<IEmployeeDbContext>();
            mock.Setup(x => x.Employees.Add(It.IsAny<EmployeeModel>()))
                .Returns(new EmployeeModel());
            mock.Setup(x => x.SaveChanges())
                .Returns(0);

            var controller = new EmployeeController(mock.Object, helperMock.Object);
            var actionResult = controller.Add(_testEmployeeModel);

            mock.Verify(x => x.Employees.Add(It.IsAny<EmployeeModel>()));
            mock.Verify(x => x.SaveChanges());
            helperMock.Verify(x => x.FindEmployees(It.IsAny<EmployeeModel>()));
        }

        [Test]
        public void TestPostAdd_InputDataSet_OkResultWithEmployeeModel()
        {
            var helperMock = new Mock<EmployeeControllerHelper>();
            helperMock.Setup(x => x.FindEmployees(It.IsAny<EmployeeModel>())).
                Returns(() => null);
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Employees, _testEmployeeDbSet);

            var controller = new EmployeeController(mock.Object, helperMock.Object);
            var expected = new OkNegotiatedContentResult<EmployeeModel>(_testEmployeeModel, controller);

            var actionResult = controller.Add(_testEmployeeModel);
            var actual = actionResult as OkNegotiatedContentResult<EmployeeModel>;

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Content);

            var compareLogic = new CompareLogic(_config);
            var comparationResult = compareLogic.Compare(expected.Content, actual.Content);

            Assert.IsTrue(comparationResult.AreEqual, comparationResult.DifferencesString);
        }

        [Test]
        public void TestGetUpdate()
        {
            var mock = new Mock<IEmployeeDbContext>();
            mock.Setup(x => x.Employees.Find(It.IsAny<int>()))
                .Returns(_testEmployeeModel);

            mock.Setup(x => x.MarkAsModified(It.IsAny<EmployeeModel>()));
            mock.Setup(x => x.SaveChanges());

            var controller = new EmployeeController(mock.Object);
            var expected = new OkNegotiatedContentResult<EmployeeModel>(_testEmployeeModel, controller);

            var actionResult = controller.Update(1, _testEmployeeModel.FirstName, _testEmployeeModel.LastName,
                _testEmployeeModel.Age.ToString(), _testEmployeeModel.Gender);
            var actual = actionResult as OkNegotiatedContentResult<EmployeeModel>;

            mock.Verify(x => x.Employees.Find(It.IsAny<int>()));
            mock.Verify(x => x.MarkAsModified(It.IsAny<EmployeeModel>()));
            mock.Verify(x => x.SaveChanges());

            Assert.IsNotNull(actual);
            var compareLogic = new CompareLogic(_config);
            var comparationResult = compareLogic.Compare(expected.Content, actual.Content);

            Assert.IsTrue(comparationResult.AreEqual, comparationResult.DifferencesString);
        }

        [Test]
        public void TestGetUpdate_Id_Not_Found()
        {
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Employees, _testEmployeeDbSet);
            mock.Setup(x => x.Employees.Find(It.Is<int>(id => id == 1)))
                .Returns(_testEmployeeModel);
            var controller = new EmployeeController(mock.Object);
            var actionResult = controller.Update(4);

            Assert.IsTrue(actionResult is NotFoundResult, "should return NotFoundResult");
        }

        [Test]
        public void TestGetRemove()
        {
            var mock = new Mock<IEmployeeDbContext>();
            mock.SetupProperty(x => x.Employees, _testEmployeeDbSet);
            mock.Setup(x => x.Employees.Find(It.Is<int>(id => id == 2)))
                .Returns(_testEmployeeModel2);
            mock.Setup(x => x.FindModelById(It.Is<int>(id => id == 2)))
                .Returns(_testEmployeeModel2);
            mock.Setup(x => x.Employees.Remove(It.Is<EmployeeModel>(model => model == _testEmployeeModel2)))
                .Callback(() => _testEmployeeDbSet.Remove(_testEmployeeModel2));
            mock.Setup(x => x.SaveChanges());

            var controller = new EmployeeController(mock.Object);
            var actionResult = controller.Remove(2);
            var actual = actionResult as OkResult;
            var expectedDbSet = new TestDbSet<EmployeeModel> { _testEmployeeModel };
            var actualDbSet = _testEmployeeDbSet;

            mock.Verify(x => x.Employees.Find(It.IsAny<int>()));
            mock.Verify(x => x.FindModelById(It.IsAny<int>()));
            mock.Verify(x => x.Employees.Remove(It.IsAny<EmployeeModel>()));
            mock.Verify(x => x.SaveChanges());

            Assert.IsNotNull(actual);
            var compareLogic = new CompareLogic(_config);
            var comparationResult = compareLogic.Compare(expectedDbSet, actualDbSet);

            Assert.IsTrue(comparationResult.AreEqual, comparationResult.DifferencesString);
        }

        [Test]
        public void TestPostAddProfession()
        {
            var newProfessions = new List<string> { "testProfOne", "testProfTwo" };
            var expected = new EmployeeModel()
            {
                Age = _testEmployeeModel.Age,
                FirstName = _testEmployeeModel.FirstName,
                LastName = _testEmployeeModel.LastName,
                Professions = new List<ProfessionModel>(_testEmployeeModel.Professions)
            };
            newProfessions.ForEach(n => expected.Professions.Add(new ProfessionModel { ProfessionName = n }));

            var mock = new Mock<IEmployeeDbContext>();
            mock.Setup(x => x.FindModelById(It.Is<int>(id => id == 1)))
                .Returns(_testEmployeeModel);
            mock.Setup(x => x.SaveChanges());

            var controller = new EmployeeController(mock.Object);
            var actionResult = controller.AddProfession(new NewProfessionsDto {Id =1, ProfessionNames = newProfessions});
            var actual = actionResult as OkNegotiatedContentResult<EmployeeModel>;

            mock.Verify(x => x.FindModelById(It.IsAny<int>()));
            mock.Verify(x => x.SaveChanges());

            Assert.IsNotNull(actual);
            var compareLogic = new CompareLogic(_config);
            var comparationResult = compareLogic.Compare(expected, actual.Content);

            Assert.IsTrue(comparationResult.AreEqual, comparationResult.DifferencesString);
        }

        [Test]
        public void TestPostFindEmployee_FindEmployees_Called()
        {
            var dbMock = new Mock<IEmployeeDbContext>();
            var helperMock = new Mock<EmployeeControllerHelper>();
            helperMock.Setup(x => x.FindEmployees(It.IsAny<EmployeeModel>()));

            var controller = new EmployeeController(dbMock.Object, helperMock.Object);
            var actionResult = controller.FindEmployee(_testEmployeeModel);
            helperMock.Verify(x => x.FindEmployees(It.IsAny<EmployeeModel>()));
        }


        private static TestDbSet<EmployeeModel> _testEmployeeDbSet;
        private static TestDbSet<ProfessionModel> _testProfessionDbSet;
        private static EmployeeModel _testEmployeeModel;
        private static EmployeeModel _testEmployeeModel2;
        private static ProfessionModel _testProfModel;
        private static ComparisonConfig _config;
    }
}
