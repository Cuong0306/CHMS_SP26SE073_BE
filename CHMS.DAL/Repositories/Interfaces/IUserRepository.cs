using CHMS.DAL.Common;
using CHMS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Sau này muốn tìm user theo Email hay gì đó thì viết thêm hàm ở đây
        // Nhưng hiện tại GenericRepository đã có sẵn GetAsync rồi nên để trống cũng được
    }
}
