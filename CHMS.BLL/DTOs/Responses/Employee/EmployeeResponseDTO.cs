using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Responses.Employee
{
    public class EmployeeResponseDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Active"; // Active/Banned
        public Guid? AssignedHomestayId { get; set; } // Homestay được phân công
        public DateTime CreatedAt { get; set; }
    }
}
