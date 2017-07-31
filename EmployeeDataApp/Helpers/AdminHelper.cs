using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using EmployeeDataApp.Extensions;
using EmployeeDataApp.Models;

namespace EmployeeDataApp.Helpers
{
    public class AdminHelper : EmployeeControllerHelper
    {
        public override List<EmployeeModel> FindEmployees(EmployeeModel searchModel)
        {
            List<EmployeeModel> employees = null;
            using (var connection = new SqlConnection(WebConfigurationManager.AppSettings["ConnectionString"]))
            {
                var findEmployeeCommand = CreateFindEmployeeCommand(connection, searchModel);
                connection.Open();
                SendFindEmployeeCommand(findEmployeeCommand, ref employees);
            }
            return FormOutputData(employees);
        }

        private SqlCommand CreateFindEmployeeCommand(SqlConnection connection, EmployeeModel searchModel)
        {
            var professions = new DataTable();
            professions.Columns.Add("ProfessionName", typeof(string));
            foreach (var profModel in searchModel.Professions)
                professions.Rows.Add(profModel.ProfessionName);

            string findEmployeeProcedure = "FindEmployeeProcedure";
            var inFirstName = new SqlParameter { ParameterName = "@firstName", Value = (object)searchModel.FirstName ?? DBNull.Value, Direction = ParameterDirection.Input };
            var intLastName = new SqlParameter { ParameterName = "@lastName", Value = (object)searchModel.LastName ?? DBNull.Value, Direction = ParameterDirection.Input };
            var inAge = new SqlParameter { ParameterName = "@age", Value = searchModel.Age, Direction = ParameterDirection.Input };
            var inGender = new SqlParameter { ParameterName = "@gender", Value = (object)searchModel.Gender ?? DBNull.Value, Direction = ParameterDirection.Input };
            var inProfessions = new SqlParameter { ParameterName = "@professions", Value = professions, SqlDbType = SqlDbType.Structured };

            var findEmployeeCommand = new SqlCommand(findEmployeeProcedure, connection);
            findEmployeeCommand.CommandType = CommandType.StoredProcedure;
            findEmployeeCommand.Parameters.AddRange(new SqlParameter[]
            {
                inFirstName, intLastName, inAge, inGender, inProfessions
            });

            return findEmployeeCommand;
        }
        private void SendFindEmployeeCommand(SqlCommand findEmployeeCommand, ref List<EmployeeModel> employees)
        {
            using (var reader = findEmployeeCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    employees = new List<EmployeeModel>();
                    while (reader.Read())
                    {
                        var employeeModel = new EmployeeModel
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            LastName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Age = reader.GetInt32(3),
                            Gender = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                        if (!reader.IsDBNull(5))
                            employeeModel.Professions.Add(new ProfessionModel
                            {
                                ProfessionName = reader.GetString(5),
                                EmployeeModelId = reader.GetInt32(6)
                            });
                        employees.Add(employeeModel);
                    }
                }
            }
        }

        private List<EmployeeModel> FormOutputData(List<EmployeeModel> employees)
        {
            var grEmployees = employees?.GroupBy(e => e.Id, e => e.Professions).ToList();

            var mappedEmployees = grEmployees?.Join(employees, g => g.Key, e => e.Id, (g, e) => new EmployeeModel
            {
                Age = e.Age,
                FirstName = e.FirstName,
                Gender = e.Gender,
                Id = e.Id,
                LastName = e.LastName,
                Professions = g.SelectMany(col => col).ToList()
            });
            return mappedEmployees?.Unique(m => m.Id).ToList();
        }
    }
}