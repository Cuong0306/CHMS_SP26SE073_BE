using CHMS.BLL.DTOs.Requests.Amenity;
using CHMS.BLL.DTOs.Responses.Amenity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IAmenityService
    {
        Task<IEnumerable<AmenityResponseDTO>> GetAllAmenitiesAsync();
        Task<AmenityResponseDTO?> GetAmenityByIdAsync(Guid id);
        Task CreateAmenityAsync(AmenityRequestDTO request);
        Task UpdateAmenityAsync(Guid id, AmenityRequestDTO request);
        Task DeleteAmenityAsync(Guid id);
    }
}
