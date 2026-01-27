using AutoMapper;
using CHMS.BLL.DTOs.Requests;
using CHMS.BLL.DTOs.Responses;
using CHMS.DAL.Entities;

namespace CHMS.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =========================================================
            // 1. Map từ Register Request (DTO) -> User (Entity)
            // =========================================================
            CreateMap<RegisterRequestDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password xử lý riêng
                                                                           // 👇 THÊM DÒNG QUAN TRỌNG NÀY:
                                                                           // AutoMapper sẽ lấy dữ liệu từ 'PhoneNumber' của DTO bỏ vào cột 'Phone' của Entity
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));


            // =========================================================
            // 2. Map từ User (Entity) -> User Response (DTO)
            // =========================================================
            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => "Customer"))
                // 👇 THÊM DÒNG QUAN TRỌNG NÀY:
                // Lấy dữ liệu từ cột 'Phone' trong DB trả về field 'PhoneNumber' cho Frontend
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));
        }
    }
}