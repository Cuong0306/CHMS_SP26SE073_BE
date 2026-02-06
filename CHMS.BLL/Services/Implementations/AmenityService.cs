using CHMS.BLL.DTOs.Requests.Amenity;
using CHMS.BLL.DTOs.Responses.Amenity;
using CHMS.BLL.Services.Interfaces;
using CHMS.DAL.Common;
using CHMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Implementations
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AmenityResponseDTO>> GetAllAmenitiesAsync()
        {
            var amenities = await _unitOfWork.Amenities.GetAllAsync();
            return amenities.Select(a => new AmenityResponseDTO
            {
                Id = a.Id,
                Name = a.Name,
                Category = a.Category,
                IconUrl = a.IconUrl
            });
        }

        public async Task<AmenityResponseDTO?> GetAmenityByIdAsync(Guid id)
        {
            var amenity = await _unitOfWork.Amenities.GetByIdAsync(id);
            if (amenity == null) return null;

            return new AmenityResponseDTO
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Category = amenity.Category,
                IconUrl = amenity.IconUrl
            };
        }

        public async Task CreateAmenityAsync(AmenityRequestDTO request)
        {
            var amenity = new Amenity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Category = request.Category,
                IconUrl = request.IconUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Amenities.AddAsync(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAmenityAsync(Guid id, AmenityRequestDTO request)
        {
            var amenity = await _unitOfWork.Amenities.GetByIdAsync(id);
            if (amenity == null) throw new Exception("Amenity not found");

            amenity.Name = request.Name;
            amenity.Category = request.Category;
            amenity.IconUrl = request.IconUrl;
            amenity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Amenities.Update(amenity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAmenityAsync(Guid id)
        {
            await _unitOfWork.Amenities.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
