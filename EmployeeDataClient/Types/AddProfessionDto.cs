using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmployeeDataClient.Types
{
    public class AddProfessionDto
    {
        [JsonProperty("Id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("ProfessionNames", Required = Required.Always)]
        public ICollection<string> ProfessionNames { get; set; }
    }
}
