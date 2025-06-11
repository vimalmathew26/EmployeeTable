using System.ComponentModel.DataAnnotations;

namespace EmployeeTable.Models
{
    public class Admin
    {
        public int id { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }
    }
}
