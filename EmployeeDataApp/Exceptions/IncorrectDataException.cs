using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace EmployeeDataApp.Exceptions
{

    [Serializable]
    public class IncorrectDataException : Exception
    {
        public IncorrectDataException(string parameterName)
        {
            _name = parameterName;
        }

        public IncorrectDataException(string parameterName, ModelStateDictionary modelState)
        {
            _name = parameterName;
            ModelState = modelState;
            Errors = modelState.ToDictionary(
                kvp => kvp.Key.Split('.').Last(),
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        }

        private readonly string _name;

        public override string Message => $"The parameter {_name} was entered incorrectly";

        public ModelStateDictionary ModelState { get; set; }

        public Dictionary<string, string[]> Errors { get; }
    }

    
}