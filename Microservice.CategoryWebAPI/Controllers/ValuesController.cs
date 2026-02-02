using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.CategoryWebAPI.Controllers;

[ApiController]
[ApiVersion(1, Deprecated = true)]
[ApiVersion(2)]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] //http://localhost:5002/api/Values?api-version=2
public class ValuesController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion(1)]
    public IActionResult GetTodo()
    {
        return Ok(new { Message = "V1 Todo..." });
    }

    [HttpGet]
    [MapToApiVersion(2)]
    public IActionResult GetTodoV2()
    {
        return Ok(new { Message = "V2 Todo..." });
    }
}