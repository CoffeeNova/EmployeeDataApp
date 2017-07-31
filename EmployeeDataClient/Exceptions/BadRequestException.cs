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
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {
            Message = base.Message;
        }

        public BadRequestException(string json) : base(json)
        {
            var badRequest = JsonConvert.DeserializeObject<BadRequestDto>(json);
            if (badRequest.Error != null)
            {
                Message = $"{badRequest.Error} - {badRequest.ErrorDescription}";
                return;
            }
            var stringBuilder = new StringBuilder(badRequest.Message);
            stringBuilder.AppendLine();
            if (badRequest.ModelState != null)
                foreach (var kvp in badRequest.ModelState)
                {
                    foreach (var value in kvp.Value)
                    {
                        stringBuilder.AppendLine(value);
                    }
                }
            Message = stringBuilder.ToString();
        }

        public BadRequestException(string json, Exception inner) : base(json, inner)
        {
            var badRequest = JsonConvert.DeserializeObject<BadRequestDto>(json);
            var stringBuilder = new StringBuilder();
            foreach (var kvp in badRequest.ModelState)
            {
                foreach (var value in kvp.Value)
                {
                    stringBuilder.AppendLine(value);
                }
            }
            Message = stringBuilder.ToString();
        }

        protected BadRequestException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public override string Message { get; }
    }

}
