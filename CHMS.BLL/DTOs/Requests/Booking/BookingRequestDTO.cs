using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Requests.Booking
{
    public class BookingRequestDTO
    {
        [Required]
        public Guid HomestayId { get; set; }

        [Required]
        public DateOnly CheckIn { get; set; } // Dùng DateOnly theo entity

        [Required]
        public DateOnly CheckOut { get; set; }

        [Range(1, 100)]
        public int GuestsCount { get; set; }

        public string? SpecialRequests { get; set; } // Đổi tên cho khớp entity

        [Phone]
        public string? ContactPhone { get; set; } // Thêm sđt liên hệ

        public Guid? PromotionId { get; set; } // Để null nếu không có mã giảm giá
    }
}
