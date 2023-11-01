using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static WebApi.Features.Resumes.AddResume;

namespace WebApi.Controllers;

[ApiController, Route("api/resume")]
public class ResumeController : ControllerBase
{
    [HttpPost("create")]
    [SwaggerOperation("Создание резюме", "Позволяет создать резюме для расмотрение")]
    [SwaggerResponse(200, "Успешно создано и отправлено")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> Create(IMediator mediator, Command command)
    {
        var result = await mediator.Send(command);

        if (result.IsError)
            return BadRequest(result);

        return Ok(result);
    }
}
