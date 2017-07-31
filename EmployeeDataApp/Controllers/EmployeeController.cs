using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using EmployeeDataApp.Comparers;
using EmployeeDataApp.Exceptions;
using EmployeeDataApp.Filters;
using EmployeeDataApp.Models;
using EmployeeDataApp.Extensions;
using EmployeeDataApp.Helpers;

namespace EmployeeDataApp.Controllers
{
    [Authorize(Roles = "Admin, Client")]
    [Exception]
    public class EmployeeController : ApiController
    {
        public EmployeeController()
        {
            db = new EmployeeDbContext();
        }

        //for tests only
        public EmployeeController(IEmployeeDbContext context)
        {
            db = context
                ?? new EmployeeDbContext();
        }

        //for tests only
        public EmployeeController(IEmployeeDbContext context, EmployeeControllerHelper helper)
        {
            db = context
                 ?? new EmployeeDbContext();
            _helper = helper;
        }

        // GET, POST: api/Employee/All
        [AcceptVerbs("GET", "POST")]
        [ResponseType(typeof(IQueryable<EmployeeModel>))]
        public IHttpActionResult All()
        {
            var collection = db.Employees
                .Include(model => model.Professions);

            return Ok(collection);
        }

        // GET, POST: api/Employee/Porfessions
        [AcceptVerbs("GET", "POST")]
        [ResponseType(typeof(DbSet<ProfessionModel>))]
        public IHttpActionResult Professions()
        {
            return Ok(db.Professions);
        }

        // GET: api/Employee/Add
        [HttpGet]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult Add(string firstName, string lastName, string age, string gender = null)
        {
            var ageInt = 0;
            if (!EmployeeControllerHelper.NameValidator(firstName))
                throw new IncorrectDataException(nameof(firstName));
            if (!EmployeeControllerHelper.NameValidator(lastName))
                throw new IncorrectDataException(nameof(lastName));
            if (!EmployeeControllerHelper.AgeValidator(age, ref ageInt))
                throw new IncorrectDataException(nameof(age));
            if (gender != null && !EmployeeControllerHelper.IsGenderFromSexParty(gender))
                throw new IncorrectDataException(nameof(gender));

            var findResult = FindEmployee(firstName, lastName, age) as OkNegotiatedContentResult<List<EmployeeModel>>;
            if (findResult?.Content?.Count > 0)
                throw new InvalidOperationException("Employee with such data already exists in the database");

            var employeeModel = db.Employees.Add(new EmployeeModel
            {
                Age = ageInt,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender
            });

            db.SaveChanges();
            return Ok(employeeModel);
        }

        // POST: api/Employee/Add
        [HttpPost]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult Add(EmployeeModel employeeModel)
        {
            if (!ModelState.IsValid)
                throw new IncorrectDataException(nameof(employeeModel), ModelState);
            var findResult = FindEmployee(employeeModel) as OkNegotiatedContentResult<List<EmployeeModel>>;
            if (findResult?.Content?.Count > 0)
                throw new InvalidOperationException("Employee with such data already exists in the database");

            var model = db.Employees.Add(employeeModel);
            db.SaveChanges();
            return Ok(model);
        }

        // GET: api/Employee/Update
        [HttpGet]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult Update(int id, string newFirstName = null, string newLastName = null, string newAge = null, string newgender = null)
        {
            var newAgeInt = 0;
            if (newFirstName != null && !EmployeeControllerHelper.IsStringOnlyLetters(newFirstName))
                throw new IncorrectDataException(nameof(newFirstName));
            if (newLastName != null && !EmployeeControllerHelper.IsStringOnlyLetters(newLastName))
                throw new IncorrectDataException(nameof(newLastName));
            if (newgender != null && !EmployeeControllerHelper.IsGenderFromSexParty(newgender))
                throw new IncorrectDataException(nameof(newgender));
            if (newAge != null && !EmployeeControllerHelper.IsAgeInRange(newAge, out newAgeInt))
                throw new IncorrectDataException(nameof(newAge));


            var employeeModel = db.Employees.Find(id);
            if (employeeModel == null)
                return NotFound();

            if (newFirstName != null)
                employeeModel.FirstName = newFirstName;
            if (newLastName != null)
                employeeModel.LastName = newLastName;
            if (newAge != null)
                employeeModel.Age = newAgeInt;
            if (newgender != null)
                employeeModel.Gender = newgender;

            db.MarkAsModified(employeeModel);
            db.SaveChanges();
            return Ok(employeeModel);
        }


        // PUT, POST: api/Employee/Update
        [AcceptVerbs("POST", "PUT")]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult Update(EmployeeModel newEmployeeModel)
        {
            if (newEmployeeModel == null)
                throw new IncorrectDataException(nameof(newEmployeeModel));
            if (!ModelState.IsValid)
                throw new IncorrectDataException(nameof(newEmployeeModel), ModelState);

            return UpdateRecord(newEmployeeModel);
        }

        // GET, POST: api/Employee/Remove
        [AcceptVerbs("GET", "POST")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Remove(int id)
        {
            var employeeModel = db.Employees.Find(id);
            if (employeeModel == null)
                return NotFound();

            return Remove(employeeModel);
        }


        // DELETE, POST: api/Employee/Remove
        [AcceptVerbs("POST", "DELETE")]
        [ResponseType(typeof(void))]
        [NonAction]
        public IHttpActionResult Remove(EmployeeModel employeeModel)
        {
            if (employeeModel == null)
                throw new IncorrectDataException(nameof(employeeModel));
            if (!ModelState.IsValid)
                throw new IncorrectDataException(nameof(employeeModel), ModelState);

            var existModel = db.FindModelById(employeeModel.Id);
            if (existModel == null)
                return NotFound();

            db.Employees.Remove(existModel);
            db.SaveChanges();

            return Ok();
        }

        //GET: api/Employee/AddProfession
        [HttpGet]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult AddProfession(int id, string professionName)
        {
            if (!EmployeeControllerHelper.NameValidator(professionName))
                throw new IncorrectDataException(nameof(professionName));

            var professions = new NewProfessionsDto
            {
                Id = id,
                ProfessionNames = new string[] {professionName}
            };
            return AddProfession(professions);
        }

        //POST: api/Employee/AddProfession
        [HttpPost]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult AddProfession(NewProfessionsDto professions)
        {
            if(!ModelState.IsValid)
            throw new IncorrectDataException(nameof(professions), ModelState);

            var existModel = db.FindModelById(professions.Id);
            if (existModel == null)
                return NotFound();

            var newProfessionList = professions.ProfessionNames
                .Select(p => new ProfessionModel { ProfessionName = p })
                .ToList();
            AddProfessions(newProfessionList, existModel.Professions);
            db.SaveChanges();
            return Ok(existModel);
        }

        //GET: api/Employee/RemoveProfession
        [HttpGet]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult RemoveProfession(int id, string professionName)
        {
            if (!EmployeeControllerHelper.NameValidator(professionName))
                throw new IncorrectDataException(nameof(professionName));

            return RemoveProfession(id, new List<string> { professionName });
        }

        //POST: api/Employee/RemoveProfession
        [HttpPost]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult RemoveProfession(int id, ICollection<string> professionNames)
        {
            if (professionNames.Any(p => !EmployeeControllerHelper.NameValidator(p)))
                throw new IncorrectDataException(nameof(professionNames));

            var existModel = db.FindModelById(id);
            if (existModel == null)
                return NotFound();

            var removeProfessionList = professionNames
                .Select(p => new ProfessionModel { ProfessionName = p, EmployeeModelId = id })
                .ToList();
            var intersectProfessions = existModel.Professions.Intersect(removeProfessionList,
                new ProfessionNameEqualityComparer<ProfessionModel>());
            db.Professions.RemoveRange(intersectProfessions);
            db.SaveChanges();
            return Ok(existModel);
        }

        // GET: api/Employee/Find
        [HttpGet, Exception, ActionName("Find")]
        [ResponseType(typeof(IEnumerable<EmployeeModel>))]
        public IHttpActionResult FindEmployee(string firstName = null, string lastName = null, string age = null,
            string gender = null, string profession = null)
        {
            var ageInt = 0;
            if (firstName != null && !EmployeeControllerHelper.IsStringOnlyLetters(firstName))
                throw new IncorrectDataException(nameof(firstName));
            if (lastName != null && !EmployeeControllerHelper.IsStringOnlyLetters(lastName))
                throw new IncorrectDataException(nameof(lastName));
            if (age != null && !EmployeeControllerHelper.IsAgeInRange(age, out ageInt))
                throw new IncorrectDataException(nameof(age));
            if (gender != null && !EmployeeControllerHelper.IsGenderFromSexParty(gender))
                throw new IncorrectDataException(nameof(lastName));

            var searchModel = new EmployeeModel
            {
                Age = ageInt,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Professions = new List<ProfessionModel>
                {
                    new ProfessionModel {ProfessionName = profession}
                }
            };
            return FindEmployee(searchModel);
        }

        // POST: api/Employee/Find
        [HttpPost, Exception, ActionName("Find")]
        [ResponseType(typeof(IEnumerable<EmployeeModel>))]
        public IHttpActionResult FindEmployee(EmployeeModel employeeModel)
        {
            //if (!ModelState.IsValid)
            //    throw new IncorrectDataException(ModelState);
            if (_helper == null)
                _helper = HelperFactory.Create(User, db);
            return Ok(_helper.FindEmployees(employeeModel));
        }

        // GET: api/Employee/FindById
        [HttpGet, Exception]
        [ResponseType(typeof(EmployeeModel))]
        public IHttpActionResult FindById(int id)
        {
            var model = db.FindModelById(id);
            return Ok(model);
        }

        private IHttpActionResult UpdateRecord(EmployeeModel newModel)
        {
            var existModel = db.FindModelById(newModel.Id);

            if (existModel == null)
                return NotFound();

            //Update
            db.Entry(existModel).CurrentValues.SetValues(newModel);
            //Update related
            db.Professions.RemoveRange(existModel.Professions.Except(newModel.Professions,
                new ProfessionNameEqualityComparer<ProfessionModel>()));
            AddProfessions(existModel.Professions, newModel.Professions);
            db.SaveChanges();
            return Ok(existModel);
        }

        private void AddProfessions(ICollection<ProfessionModel> newProfessions,
            ICollection<ProfessionModel> existProfessions)
        {
            var tempCollection = existProfessions.Select(i => i).ToList();
            foreach (var profession in newProfessions)
            {
                var existProfessionsModel = tempCollection
                    .SingleOrDefault(p => p.ProfessionName == profession.ProfessionName);
                if (existProfessionsModel != null)
                    continue;

                var newProfession = new ProfessionModel
                {
                    ProfessionName = profession.ProfessionName
                };
                existProfessions.Add(newProfession);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeModelExists(string id)
        {
            return db.Employees.Count(e => e.FirstName == id) > 0;
        }

        private EmployeeControllerHelper _helper;
        private IEmployeeDbContext db;
        private delegate bool CompareDelegate<in T>(T obj1, T obj2);


    }
}