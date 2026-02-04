using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Responses.Homestay
{
    public class HomestayResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public string Status { get; set; } = string.Empty;

        // Location info flatten (làm phẳng) cho dễ hiển thị
        public string Address { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public List<string> ImageUrls { get; set; } = new();
        public List<string> AmenityNames { get; set; } = new();
    }
}
