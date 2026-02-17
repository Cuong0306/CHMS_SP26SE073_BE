using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Requests.Employee
{
    public class CreateEmployeeRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty; // Admin đặt mật khẩu ban đầu

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Url]
        public string AvatarUrl { get;  set; } = string.Empty;
    }
}
