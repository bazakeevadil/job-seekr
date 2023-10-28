using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Contract;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Enums;
using static WebApi.Features.Auth.RegisterUser;

namespace WebApi.Features.Auth;

[ApiController, Route("api/auth")]
public class Controller : ControllerBase
{
    [HttpPost("register")]
    [SwaggerOperation("Регистрация", "Позволяет зарегистрироватся и получить доступ для просмотра")]
    [SwaggerResponse(200, "Успешно получено")]
    [SwaggerResponse(400, "Ошибка валидации")]
    public async Task<IActionResult> Register(Command command,IMediator mediator)
    {
        if (command.Email.IsNullOrEmpty()) return BadRequest("Email is null");
        if (command.Password.IsNullOrEmpty()) return BadRequest("Password is null");

        var response = await mediator.Send(command);

        return Ok(response);
    }
}

public static class RegisterUser
{
    public record Command : IRequest<UserDto>
    {
        public required string Email { get; init; }

        public required string Password { get; init; }
    }

    internal class Handler : IRequestHandler<Command, UserDto>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                Role = Role.User
            };

            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
            };

            return response;
        }
    }
}
