using CHMS.BLL.DTOs.Requests.Homestay;
using CHMS.BLL.DTOs.Responses.Homestay;
using CHMS.BLL.Services.Interfaces;
using CHMS.DAL.Common;
using CHMS.DAL.Entities;
using CHMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Implementations
{
    public class HomestayService : IHomestayService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomestayService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddHomestayImageAsync(Guid homestayId, string imageUrl)
        {
            var img = new CHMS.DAL.Entities.HomestayImage
            {
                Id = Guid.NewGuid(),
                HomestayId = homestayId,
                ImageUrl = imageUrl,
                IsPrimary = false, // Mặc định false
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.HomestayImages.AddAsync(img);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CreateHomestayAsync(CreateHomestayRequestDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // B1: Tạo Location
                var location = new Location
                {
                    Id = Guid.NewGuid(),
                    DistrictId = dto.DistrictId,
                    Address = dto.Address,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude
                };
                await _unitOfWork.Locations.AddAsync(location);

                // B2: Tạo Homestay
                var homestay = new Homestay
                {
                    Id = Guid.NewGuid(),
                    OwnerId = dto.OwnerId,
                    LocationId = location.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    PricePerNight = dto.PricePerNight,
                    MaxGuests = dto.MaxGuests,
                    Bedrooms = dto.Bedrooms,
                    Bathrooms = dto.Bathrooms,
                    Area = dto.Area,
                    CancellationPolicy = dto.CancellationPolicy,
                    HouseRules = dto.HouseRules,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Homestays.AddAsync(homestay);

                // B3: Lưu Ảnh
                if (dto.Images != null && dto.Images.Any())
                {
                    var images = dto.Images.Select((img, index) => new HomestayImage
                    {
                        Id = Guid.NewGuid(),
                        HomestayId = homestay.Id,
                        ImageUrl = img.ImageUrl,
                        Caption = img.Caption,
                        IsPrimary = img.IsPrimary,
                        DisplayOrder = index,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();
                    await _unitOfWork.HomestayImages.AddRangeAsync(images);
                }

                // B4: Lưu Tiện Nghi
                if (dto.AmenityIds != null && dto.AmenityIds.Any())
                {
                    var homestayAmenities = dto.AmenityIds.Select(amenityId => new HomestayAmenity
                    {
                        HomestayId = homestay.Id,
                        AmenityId = amenityId
                    }).ToList();
                    await _unitOfWork.HomestayAmenities.AddRangeAsync(homestayAmenities);
                }

                // B5: Commit
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteHomestayAsync(Guid id)
        {
            var homestay = await _unitOfWork.Homestays.GetByIdAsync(id);

            if (homestay == null)
                throw new Exception("Homestay không tìm thấy để xóa.");

            await _unitOfWork.Homestays.SoftDeleteAsync(id);

            // Lưu thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteHomestayImageAsync(Guid photoId)
        {
            // Ở đây ta xóa cứng (Hard Delete) ảnh luôn vì ảnh rác không cần giữ
            var img = await _unitOfWork.HomestayImages.GetByIdAsync(photoId);
            if (img != null)
            {
                _unitOfWork.HomestayImages.Delete(img);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<HomestayResponseDTO>> GetAllHomestaysAsync()
        {
            var homestays = await _unitOfWork.Homestays.GetAllAsync(
                h => true,
                h => h.Location,
                h => h.Location.District,
                h => h.Location.District.Province,
                h => h.HomestayImages,
                h => h.HomestayAmenities, // Chỉ lấy bảng trung gian
                h => h.Owner // Owner
            );

            // Map sang DTO
            return homestays.Select(h => new HomestayResponseDTO
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                PricePerNight = h.PricePerNight,
                MaxGuests = h.MaxGuests,
                Status = h.Status,
                Address = h.Location?.Address ?? "",
                DistrictName = h.Location?.District?.Name ?? "",
                ProvinceName = h.Location?.District?.Province?.Name ?? "",
                OwnerName = h.Owner?.FullName ?? "",
                ImageUrls = h.HomestayImages?.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                // Lưu ý: Để lấy tên Amenity, query GetAllAsync ở trên cần tối ưu lại hoặc chấp nhận lazy loading
                // Ở danh sách tổng quan ta có thể tạm bỏ qua AmenityNames để query nhẹ hơn
                AmenityNames = new List<string>()
            });
        }

        public async Task<HomestayResponseDTO?> GetHomestayByIdAsync(Guid id)
        {
            var homestay = await _unitOfWork.Homestays.GetAsync(
                h => h.Id == id,
                h => h.Location,
                h => h.Location.District,
                h => h.Location.District.Province,
                h => h.HomestayImages,
                h => h.HomestayAmenities,
                h => h.Owner
            );

            if (homestay == null) return null;

            // Lấy tên các tiện nghi
            var amenityNames = new List<string>();
            if (homestay.HomestayAmenities != null)
            {
                foreach (var ha in homestay.HomestayAmenities)
                {
                    var amenity = await _unitOfWork.Amenities.GetByIdAsync(ha.AmenityId);
                    if (amenity != null) amenityNames.Add(amenity.Name);
                }
            }

            return new HomestayResponseDTO
            {
                Id = homestay.Id,
                Name = homestay.Name,
                Description = homestay.Description,
                PricePerNight = homestay.PricePerNight,
                MaxGuests = homestay.MaxGuests,
                Status = homestay.Status,
                Address = homestay.Location?.Address ?? "",
                DistrictName = homestay.Location?.District?.Name ?? "",
                ProvinceName = homestay.Location?.District?.Province?.Name ?? "",
                OwnerName = homestay.Owner?.FullName ?? "",
                ImageUrls = homestay.HomestayImages?.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                AmenityNames = amenityNames
            };
        }

        public async Task ReorderHomestayImagesAsync(Guid homestayId, List<Guid> sortedImageIds)
        {
            // 1. Lấy tất cả ảnh của homestay này từ DB
            var existingImages = await _unitOfWork.HomestayImages.GetAllAsync(x => x.HomestayId == homestayId);
            var imageDict = existingImages.ToDictionary(x => x.Id);

            // 2. Duyệt qua danh sách ID gửi lên để cập nhật thứ tự
            for (int i = 0; i < sortedImageIds.Count; i++)
            {
                var imageId = sortedImageIds[i];
                if (imageDict.ContainsKey(imageId))
                {
                    var img = imageDict[imageId];
                    img.DisplayOrder = i; // 0, 1, 2...

                    // Logic phụ: Ảnh đầu tiên trong list (index 0) sẽ tự động là Ảnh Chính (IsPrimary)
                    img.IsPrimary = (i == 0);

                    _unitOfWork.HomestayImages.Update(img);
                }
            }

            // 3. Lưu thay đổi
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateHomestayAmenitiesAsync(Guid homestayId, List<Guid> amenityIds)
        {
            var homestay = await _unitOfWork.Homestays.GetByIdAsync(homestayId);
            if (homestay == null) throw new Exception("Homestay not found");

            // Xóa hết cái cũ
            var current = await _unitOfWork.HomestayAmenities.GetAllAsync(x => x.HomestayId == homestayId);
            if (current.Any()) _unitOfWork.HomestayAmenities.DeleteRange(current);

            // Thêm cái mới
            if (amenityIds != null && amenityIds.Any())
            {
                var newAmenities = amenityIds.Select(aid => new HomestayAmenity
                {
                    HomestayId = homestayId,
                    AmenityId = aid
                });
                await _unitOfWork.HomestayAmenities.AddRangeAsync(newAmenities);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateHomestayAsync(Guid id, UpdateHomestayRequestDTO dto)
        {
            var homestay = await _unitOfWork.Homestays.GetByIdAsync(id);
            if (homestay == null) throw new Exception("Homestay không tồn tại.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // A. Update bảng Homestay
                homestay.Name = dto.Name;
                homestay.Description = dto.Description;
                homestay.PricePerNight = dto.PricePerNight;
                homestay.MaxGuests = dto.MaxGuests;
                homestay.Bedrooms = dto.Bedrooms;
                homestay.Bathrooms = dto.Bathrooms;
                homestay.Area = dto.Area;
                homestay.CancellationPolicy = dto.CancellationPolicy;
                homestay.HouseRules = dto.HouseRules;
                // homestay.UpdatedAt = DateTime.UtcNow; // Đã xử lý tự động ở Context

                _unitOfWork.Homestays.Update(homestay);

                // B. Update bảng Location
                var location = await _unitOfWork.Locations.GetByIdAsync(homestay.LocationId);
                if (location != null)
                {
                    location.Address = dto.Address;
                    location.DistrictId = dto.DistrictId;
                    location.Latitude = dto.Latitude;
                    location.Longitude = dto.Longitude;
                    _unitOfWork.Locations.Update(location);
                }

                // C. Xử lý Amenities (Xóa cũ - Thêm mới)
                var currentAmenities = await _unitOfWork.HomestayAmenities.GetAllAsync(x => x.HomestayId == id);
                if (currentAmenities.Any())
                {
                    _unitOfWork.HomestayAmenities.DeleteRange(currentAmenities);
                }

                if (dto.AmenityIds != null && dto.AmenityIds.Any())
                {
                    var newAmenities = dto.AmenityIds.Select(aid => new HomestayAmenity
                    {
                        HomestayId = id,
                        AmenityId = aid
                    }).ToList();
                    await _unitOfWork.HomestayAmenities.AddRangeAsync(newAmenities);
                }

                // D. Xử lý Images (Tương tự: Xóa hết ảnh cũ - Thêm ảnh mới)
                // Lưu ý: Logic thực tế có thể phức tạp hơn nếu muốn giữ lại ảnh cũ
                var currentImages = await _unitOfWork.HomestayImages.GetAllAsync(x => x.HomestayId == id);
                if (currentImages.Any())
                {
                    _unitOfWork.HomestayImages.DeleteRange(currentImages);
                }

                if (dto.Images != null && dto.Images.Any())
                {
                    var newImages = dto.Images.Select((img, index) => new HomestayImage
                    {
                        Id = Guid.NewGuid(),
                        HomestayId = id,
                        ImageUrl = img.ImageUrl,
                        Caption = img.Caption,
                        IsPrimary = img.IsPrimary,
                        DisplayOrder = index,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();
                    await _unitOfWork.HomestayImages.AddRangeAsync(newImages);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateStatusAsync(Guid id, string newStatus)
        {
            var homestay = await _unitOfWork.Homestays.GetByIdAsync(id);
            if (homestay == null) throw new Exception("Homestay không tìm thấy");

            homestay.Status = newStatus;
            // homestay.UpdatedAt = DateTime.UtcNow; // Tự động

            _unitOfWork.Homestays.Update(homestay);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
