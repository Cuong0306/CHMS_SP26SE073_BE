using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Requests.Amenity
{
    public class AmenityRequestDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Ví dụ: Internet, Outdoor, Comfort
        public string IconUrl { get; set; } = string.Empty;
    }
}
