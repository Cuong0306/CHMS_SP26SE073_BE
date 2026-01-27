using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BadRequestException : Exception
{
    public int StatusCode { get; } = 400;
    public List<string>? Errors { get; }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, List<string> errors) : base(message)
    {
        Errors = errors;
    }
}
