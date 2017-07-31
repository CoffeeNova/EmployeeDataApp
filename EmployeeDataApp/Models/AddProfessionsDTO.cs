using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EmployeeDataApp.Attributes;
using EmployeeDataApp.Converters;
using Newtonsoft.Json;

namespace EmployeeDataApp.Models
{
    public class NewProfessionsDto
    {
        [Range(1, Int32.MaxValue, ErrorMessage = "Id must contain only positive numbers.")]
        public int Id { get; set; }

        [StringLength(20)]
        [JsonConverter(typeof(ArrayToICollectionConverter<string>))]
        public ICollection<string> ProfessionNames { get; set; }
    }
}