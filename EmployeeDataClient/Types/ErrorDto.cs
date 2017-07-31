using Newtonsoft.Json;

namespace EmployeeDataClient.Types
{
    public class ErrorDto
    {
        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("ExceptionMessage")]
        public string ExceptionMessage { get; set; }
    }
}
