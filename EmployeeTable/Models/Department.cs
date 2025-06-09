using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace EmployeeTable.Models
{
    public class Department
    {
        [Key]
        public int Deptid { get; set; }

        public string Deptname { get; set; }

    }

    public class DepartmentContext : DbContext
    { 
        public DbSet<Department> Departments {  get; set; }
    }
}