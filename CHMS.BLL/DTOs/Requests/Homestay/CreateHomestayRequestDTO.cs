using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Requests.Homestay
{
    public class CreateHomestayRequestDTO
    {
        public Guid OwnerId { get; set; } // Nếu Admin tạo hộ
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal? Area { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
        public string HouseRules { get; set; } = string.Empty;

        // Location Info
        public Guid DistrictId { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // List Amenities (Chỉ cần gửi List ID)
        public List<Guid> AmenityIds { get; set; } = new();

        // List Images
        public List<HomestayImageDTO> Images { get; set; } = new();
    }
}
