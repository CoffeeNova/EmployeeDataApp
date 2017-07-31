using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EmployeeDataClient.Exceptions;
using EmployeeDataClient.Types;
using Newtonsoft.Json;

namespace EmployeeDataClient
{
    public partial class Program
    {
#if DEBUG
        private const string DomainName = @"http://localhost:1646/";
#endif
        private static Token GetToken(string login, string password)
        {
            var content = new Content();
            content.Add("grant_type", "password");
            if (login != null)
                content.Add("username", login);
            if (password != null)
                content.Add("password", password);

            using (var form = new FormUrlEncodedContent(content.Data))
            {
                return Request<Token>("Token", RequestMethod.Post, form);
            }
        }

        private static string RegisterAction(string login, string password)
        {
            var content = new Content();
            content.Add("grant_type", "password");
            if (login != null)
                content.Add("Email", login);
            if (password != null)
                content.Add("Password", password);
            if (password != null)
                content.Add("ConfirmPassword", password);

            using (var form = new FormUrlEncodedContent(content.Data))
            {
                return Request<string>("api/Account/Register", RequestMethod.Post, form);
            }
        }


        private static List<EmployeeDto> GetAllAction()
        {
            return Request<List<EmployeeDto>>("api/Employee/All", RequestMethod.Get);
        }

        private static EmployeeDto AddAction(EmployeeDto employee)
        {
            var payload = JsonConvert.SerializeObject(employee, Settings);
            using (var httpContent = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
            {
                return Request<EmployeeDto>("api/Employee/Add", RequestMethod.Post, httpContent);
            }
        }

        private static AddProfessionDto AddProfessionAction(AddProfessionDto professions)
        {
            var payload = JsonConvert.SerializeObject(professions, Settings);
            using (var httpContent = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
            {
                return Request<AddProfessionDto>("api/Employee/AddProfession", RequestMethod.Post, httpContent);
            }
        }

        private static List<EmployeeDto> FindAction(EmployeeDto employee)
        {
            var payload = JsonConvert.SerializeObject(employee, Settings);
            using (var httpContent = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
            {
                return Request<List<EmployeeDto>>("api/Employee/Find", RequestMethod.Post, httpContent);
            }
        }

        private static EmployeeDto FindByIdAction(string id)
        {
            var parameters = new Dictionary<string, string>
            {
                {"Id", id}
            };
            return Request<EmployeeDto>("api/Employee/FindById", RequestMethod.Get, parameters: parameters);
        }

        private static EmployeeDto UpdateAction(EmployeeDto employee)
        {
            var payload = JsonConvert.SerializeObject(employee, Settings);
            using (var httpContent = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
            {
                return Request<EmployeeDto>("api/Employee/Update", RequestMethod.Post, httpContent);
            }
        }

        private static T Request<T>(string methodPath, RequestMethod method, HttpContent content = null, Dictionary<string, string> parameters = null)
        {
            //if (method == RequestMethod.Post && content == null)
            //    throw new ArgumentNullException(nameof(content), "Must be not null for POST request");
            var response = method == RequestMethod.Post
                ? Post(content, methodPath)
                : Get(methodPath, parameters);

            if (response == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(response, Settings);
        }

        private static string Post(HttpContent content, string methodPath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (_token != null)
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _token.AccessToken);
                    var response =
                        httpClient.PostAsync(DomainName + methodPath, content).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        throw new ErrorException(response.Content.ReadAsStringAsync().Result);

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                Wline(ex.Message); //to log for future
            }
            return null;
        }

        private static string Get(string methodPath, Dictionary<string, string> parameters)
        {
            var parametersPart = new StringBuilder();
            if (parameters != null)
                foreach (var parameter in parameters)
                {
                    parametersPart.Append(parametersPart.Length == 0 ? "?" : "&");
                    parametersPart.Append($"{parameter.Key}={parameter.Value}");
                    
                }
           
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (_token != null)
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _token.AccessToken);
                    var response = httpClient.GetAsync(DomainName + methodPath + parametersPart.ToString()).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        throw new BadRequestException(response.Content.ReadAsStringAsync().Result);
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        throw new ErrorException(response.Content.ReadAsStringAsync().Result);
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                Wline(ex.Message);
            }
            return null;
        }

        private enum RequestMethod
        {
            Get,
            Post
        }

    }
}
