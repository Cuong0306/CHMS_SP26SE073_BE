using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.BLL.Common.Exceptions;

public class NotFoundException : Exception
{
    public int StatusCode { get; } = 404;

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} với ID '{key}' không tồn tại.") { }
}
