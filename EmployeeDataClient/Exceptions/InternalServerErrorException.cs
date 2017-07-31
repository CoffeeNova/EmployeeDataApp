using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataClient.Types;
using Newtonsoft.Json;

namespace EmployeeDataClient.Exceptions
{

    [Serializable]
    public class ErrorException : Exception
    {
        public ErrorException() { Message = base.Message; }

        public ErrorException(string json) : base(json)
        {
            var error = JsonConvert.DeserializeObject<ErrorDto>(json);
            Message = $"Message:{error.Message}{Environment.NewLine}Exception:{error.ExceptionMessage}";
        }

        public ErrorException(string json, Exception inner) : base(json, inner)
        {
            var error = JsonConvert.DeserializeObject<ErrorDto>(json);
            Message = error.Message;
        }
        protected ErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public override string Message { get; }

    }
}
