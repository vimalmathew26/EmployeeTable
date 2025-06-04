using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EmployeeTable.Models
{
    public class Department
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

    }
}