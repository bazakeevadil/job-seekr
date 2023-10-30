using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static WebApi.Features.Auth.LoginUser;
using static WebApi.Features.Auth.RegisterUser;

namespace WebApi.Controllers;

[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [SwaggerOperation("Для получения токена", "Позволяет получить токен для пользоваться функциями админа")]
    [SwaggerResponse(200, "Успешно получено")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> Login(IMediator mediator, Query query)
    {
        var result = await mediator.Send(query);

        if (result.IsError)
            return BadRequest(result);

        return Ok(result.Value);
    }

    [HttpPost("register")]
    [SwaggerOperation("Регистрация", "Позволяет зарегистрироватся и получить доступ для просмотра")]
    [SwaggerResponse(200, "Успешно получено")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> Register(IMediator mediator, Command command)
    {
        var result = await mediator.Send(command);

        if (result.IsError)
            return BadRequest(result);

        return Ok(result);
    }
}
