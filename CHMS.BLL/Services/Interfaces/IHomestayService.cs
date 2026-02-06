using CHMS.BLL.DTOs.Requests.Homestay;
using CHMS.BLL.DTOs.Responses.Homestay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IHomestayService
    {
        public Task<IEnumerable<HomestayResponseDTO>> GetAllHomestaysAsync();

        public Task<HomestayResponseDTO?> GetHomestayByIdAsync(Guid id);

        public Task<bool> CreateHomestayAsync(CreateHomestayRequestDTO dto);

        public Task UpdateHomestayAsync(Guid id, UpdateHomestayRequestDTO dto);

        public Task UpdateStatusAsync(Guid id, string newStatus);

        public Task DeleteHomestayAsync(Guid id);

        Task UpdateHomestayAmenitiesAsync(Guid homestayId, List<Guid> amenityIds);

        Task AddHomestayImageAsync(Guid homestayId, string imageUrl);
        Task DeleteHomestayImageAsync(Guid photoId);

        Task ReorderHomestayImagesAsync(Guid homestayId, List<Guid> sortedImageIds);
    }
}
