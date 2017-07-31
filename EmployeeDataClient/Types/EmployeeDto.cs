using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataClient.Converters;
using Newtonsoft.Json;

namespace EmployeeDataClient.Types
{
    public class EmployeeDto
    {
        [JsonProperty("Id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty("FirstName", Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty("LastName", Required = Required.Always)]
        public string LastName { get; set; }

        [JsonProperty("Age", Required = Required.Always)]
        public int Age { get; set; }

        [JsonProperty("Gender", Required = Required.Default)]
        public string Gender { get; set; }

        [JsonConverter(typeof(ArrayToICollectionConverter<ProfessionDto>))]
        [JsonProperty("Professions", Required = Required.Default)]
        public virtual ICollection<ProfessionDto> Professions { get; set; }
    }
    
}
