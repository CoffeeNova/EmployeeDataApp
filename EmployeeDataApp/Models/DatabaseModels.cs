using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using EmployeeDataApp.Attributes;
using EmployeeDataApp.Controllers;
using EmployeeDataApp.Exceptions;

namespace EmployeeDataApp.Models
{

    public class EmployeeModel : ICommonIdKey
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Index("UniqueEmployeeInd", IsUnique = true, Order = 1)]
        [StringLength(20)]
        [RequiredName(ErrorMessage = "First name must be not empty")]
        public string FirstName { get; set; }

        [Index("UniqueEmployeeInd", IsUnique = true, Order = 2)]
        [StringLength(20)]
        [RequiredName(ErrorMessage = "Last name must be not empty")]
        public string LastName { get; set; }

        [Index("UniqueEmployeeInd", IsUnique = true, Order = 3)]
        [Range(14, 99, ErrorMessage = "Age must be in range from 14 to 99")]
        public int Age { get; set; }

        [RequiredGender(ErrorMessage = "Gender must be male or female (for the moment at least)")]
        public string Gender { get; set; }

        public virtual ICollection<ProfessionModel> Professions { get; set; } = new List<ProfessionModel>();
    }

    public class ProfessionModel : IProfessionKey
    {
        [Index]
        [StringLength(20)]
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProfessionName { get; set; }

        [ForeignKey("EmployeeModelId")]
        [IgnoreDataMember]
        public virtual EmployeeModel EmployeeModel { get; set; }

        [Key, Column(Order = 1)]
        public int EmployeeModelId { get; set; }

    }
}