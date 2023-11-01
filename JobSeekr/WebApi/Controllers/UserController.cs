using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static WebApi.Features.User.DeleteUser;
using static WebApi.Features.User.GetAllUser;

namespace WebApi.Controllers;

[ApiController,Route("api/user")]
public class UserController : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Получает всех пользователей", "Позволяет получить всех пользователей. Только для админа!!!")]
    [SwaggerResponse(200, "Успешно получены")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> GetAll(IMediator mediator, Query query )
    {
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpDelete("by-email/{email}")]
    [SwaggerOperation("Удаляет пользователя по имени", "Позволяет удалить пользователя по имени. Только для админа!!!")]
    [SwaggerResponse(200, "Успешно удалено")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> Delete(IMediator mediator, Command command)
    {
        await mediator.Send(command);

        return Ok();
    }
}
