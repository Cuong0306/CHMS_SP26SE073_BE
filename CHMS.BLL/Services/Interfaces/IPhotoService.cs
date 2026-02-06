using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string> UploadPhotoAsync(IFormFile file); // Trả về URL ảnh
        Task DeletePhotoAsync(string publicId); // Xóa ảnh trên cloud (nếu cần)
    }
}
