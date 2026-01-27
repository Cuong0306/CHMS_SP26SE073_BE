using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public int StatusCode { get; } = 401;

    public UnauthorizedException(string message = "Bạn cần đăng nhập để thực hiện chức năng này.")
        : base(message) { }
}
