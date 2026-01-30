using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.DTOs.Requests
{
    public class ForgotPasswordRequestDTO
    {
        public string Email { get; set; } = string.Empty;
    }
}
