using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmployeeDataClient.Types
{
    public class ProfessionDto
    {
        [JsonProperty("ProfessionName")]
        public string Name { get; set; }

        [JsonProperty("EmployeeModelId")]
        public int Id { get; set; }
    }
}
