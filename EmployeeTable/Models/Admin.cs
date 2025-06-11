using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTable.Models
{
    public class Admin
    {
        int id {  get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}