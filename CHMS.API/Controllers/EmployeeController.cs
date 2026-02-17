using CHMS.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CHMS.API.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
    }
}
