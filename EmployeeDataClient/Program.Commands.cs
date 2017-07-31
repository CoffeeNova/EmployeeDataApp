using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeeDataClient.Types;
using System.Windows.Forms;

namespace EmployeeDataClient
{
    public partial class Program
    {
        private static void InitialMessage()
        {
            Wline("Console tool for general testing EmployeeDataApp webapi.");
            Wline(Environment.NewLine);
            Wline("Enter 'help' command to call help info.");
            Wline("Enter a command:");
        }

        private static void Help()
        {
            Wline("Currently available comands:");
            Wline("");
            Wline("login - get a new access token");
            Wline("signup - new account registration");
            Wline("all - display all employees");
            Wline("add - add new employee");
            Wline("find - find employees");
            Wline("update - update imployee information");
            Wline("");
            Wline("Use next commands any time:");
            Wline("'break', '/b', '/break' - to break current operation");
            Wline("'quit', '/q', 'exit' - to exit the program");
            Wline("'help', '/h', '/help' - to break current operation and call help info");
        }

        private static void LogIn()
        {
            Wline("Log in process started.");
#if DEBUG
            Wline("Push \"a\" to login and password for testing purposes");
#endif
            var login = EmailAddress();
            var password = Password();
#if DEBUG
            if (login.Equals("a", StringComparison.InvariantCultureIgnoreCase))
                login = "service@epam.com";
            if(password.Equals("a", StringComparison.InvariantCultureIgnoreCase))
                password = "Easypass1!";
#endif
            var token = GetToken(login, password);
            if (token != null)
            {
                Wline("You signed up!");
                Wline($"Your token is:{Environment.NewLine}{token.AccessToken}");
            }

            _token = token;
        }

        private static void SignUp()
        {
            Wline("Registration process started.");
            var login = EmailAddress();
            string password = null;
            var endloop = false;
            while (!endloop)
            {
                password = Password();
                var confirmPass = ConfirmPassword();
                if (password != confirmPass)
                    Wline("Passwords do not match!");
                else
                    endloop = true;
            }

            var result = RegisterAction(login, password);
            if (result != null)
            {
                Wline("Registration completed successfully!");
            }
        }

        private static void All()
        {
            Wline("Adding new employee process started.");

            var result = GetAllAction();
            if (result != null)
                ShowDataAsTable(result);
        }

        private static void Add()
        {
            var firstName = FirstName();
            var lastName = LastName();
            var age = Age();
            var gender = Gender();
            var professions = Professions();
            var profArr = professions?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var employee = new EmployeeDto
            {
                Age = Convert.ToInt32(age),
                FirstName = firstName,
                Gender = gender,
                LastName = lastName,
                Professions = profArr?.Select(p => new ProfessionDto
                {
                    Name = p
                }).ToList()
            };
            var result = AddAction(employee);
            if (result == null) return;
            Wline("New employee with data:");
            ShowDataAsTable(new List<EmployeeDto> { result });
            Wline("added to the database successfully");
        }

        private static void AddProfession()
        {
            var id = Id();
            var professions = Professions();
            var profArr = professions?.Split(new string[] { ",  ", ", ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var addProfDto = new AddProfessionDto
            {
                Id = id,
                ProfessionNames = profArr
            };
            var result = AddProfessionAction(addProfDto);
        }

        private static void Find()
        {
            Wline("Skip extra fields that will not participate in the search");
            var firstName = FirstName();
            var lastName = LastName();
            var age = Age();
            var gender = Gender();
            var professions = Professions();
            var profArr = professions?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var employee = new EmployeeDto
            {
                Age = string.IsNullOrEmpty(age) ? 0 : Convert.ToInt32(age),
                FirstName = string.IsNullOrEmpty(firstName) ? null : firstName,
                Gender = string.IsNullOrEmpty(gender) ? null : gender,
                LastName = string.IsNullOrEmpty(lastName) ? null : lastName,
                Professions =  profArr?.Select(p => new ProfessionDto
                {
                    Name = p
                }).ToList()
            };
            var result = FindAction(employee);
            Wline("Search results");
            ShowDataAsTable(result);
        }

        private static void Update()
        {
            var id = Id();
            var findResult = FindByIdAction(id);
            Wline("Just skip the fileds that you don't want to change.");
            var firstName = FirstName(findResult.FirstName);
            var lastName = LastName(findResult.LastName);
            var age = Age(findResult.Age.ToString());
            var gender = Gender($"Current employee gender is: {findResult.Gender}");
            var profDefaultValue = new StringBuilder();
            foreach (var prof in findResult.Professions)
                profDefaultValue.Append(prof.Name+ ',');
            profDefaultValue.Remove(profDefaultValue.Length-1, 1);
            var professions = Professions(profDefaultValue.ToString());
            var profArr = professions?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            findResult.FirstName = firstName;
            findResult.LastName = lastName;
            findResult.Age = Convert.ToInt32(age);
            findResult.Gender = string.IsNullOrEmpty(gender) ? findResult.Gender : gender;
            findResult.Professions = profArr?.Select(p => new ProfessionDto
            {
                Name = p
            }).ToList();
            var updResult = UpdateAction(findResult);
            ShowDataAsTable(new List<EmployeeDto> { updResult});
        }

        private static string EmailAddress()
        {
            var action = "Email address:   ";
            var wrong = "Email must not be empty string!";
            var func = new Func<string, bool>(s => !string.IsNullOrEmpty(s));
            return StringFunction(action, wrong, validation: func);
        }

        private static string Password()
        {
            var action = "Password:   ";
            var wrong = "Password must not be empty string!";
            var func = new Func<string, bool>(s => !string.IsNullOrEmpty(s));
            return StringFunction(action, wrong, validation: func);
        }

        private static string ConfirmPassword()
        {
            var action = "Please confirm your password:   ";
            var wrong = "password must not be empty string!";
            var func = new Func<string, bool>(s => !string.IsNullOrEmpty(s));
            return StringFunction(action, wrong, validation: func);
        }

        private static string FirstName(string defautlValue = null)
        {
            var action = "First name:   ";
            var wrong = "First name must not be empty string and must contain only latin letters!";
            var func = new Func<string, bool>(NameValidator);
            return StringFunction(action, wrong, validation: func, defaultValue: defautlValue);
        }

        private static string LastName(string defautlValue = null)
        {
            var action = "Last name:   ";
            var wrong = "Last name must not be empty string and must contain only latin letters!";
            var func = new Func<string, bool>(NameValidator);
            return StringFunction(action, wrong, validation: func, defaultValue: defautlValue);
        }

        private static string Age(string defautlValue = null)
        {
            var action = "Age:   ";
            var wrong = "Age must be at range from 14 to 99!";
            var func = new Func<string, bool>(AgeValidator);
            return StringFunction(action, wrong, validation: func, defaultValue: defautlValue);
        }

        private static string Gender(string defValueDescription = null)
        {
            var action = "Choose gender. Press [M] to choose Male, [F] to choose Female or [S] to skip this step.";
            var wrong = "Try again pls. Input should be [M] or [F]. You could press [S] to skip this step, [Q] to quit the program, [H] to help and [B] to break current operation.";
            var keyDictionary = new Dictionary<ConsoleKey, string>
            {
                {ConsoleKey.M, "Male"}, {ConsoleKey.F, "Female"}, {ConsoleKey.S, null }
            };

            return KeyFunction(keyDictionary, action, wrong, defValueDescription: defValueDescription);
        }

        private static string Professions(string defautlValue = null)
        {
            var action = "Professions (enter comma separated values for multi professions):   ";
            var wrong = "Professions must contains only latin letters!";
            var func = new Func<string, bool>(ProfessionsValidator);
            return StringFunction(action, wrong, validation: func, defaultValue: defautlValue);
        }

        private static string Id()
        {
            var action = "Id:   ";
            var wrong = "Id must contain only positive numbers!";
            var func = new Func<string, bool>(IdValidator);
            return StringFunction(action, wrong, validation: func);
        }

        private static void ShowDataAsTable(ICollection<EmployeeDto> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Wline(Environment.NewLine);
            Wline($"Employees count: {data.Count}");
            var i = 1;
            foreach (var employee in data)
            {
                var gender = employee.Gender ?? "--";
                var professions = employee.Professions == null ? "--" : string.Join(",", employee.Professions.Select(p => p.Name));

                Wline($"{i++}. {employee.FirstName} {employee.LastName}");
                Wline($"Age: {employee.Age} Gender: {gender}");
                Wline($"Professions: {professions}");
                Wline($"Employee Id: {employee.Id}");
                Wline(Environment.NewLine);
            }
        }

    }
}
