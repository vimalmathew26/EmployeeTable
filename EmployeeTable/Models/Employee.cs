using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace EmployeeTable.Models
{
    public class Employee
    {
        public int id { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int DeptId { get; set; }

        public DateTime dob { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string Deptname { get; set; }



    }
}