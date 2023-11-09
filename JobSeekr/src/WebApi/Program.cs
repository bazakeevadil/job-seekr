using Serilog;
using WebApi;
using WebApi.Shared.Behaviors;
using WebApi.Shared.Middlewere;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
   loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddMediatr();

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddAnyCors();
builder.Services.AddCarter();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingPipelineBehavior<,>));

builder.Services.AddScoped(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationPipelineBehavior<,>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapCarter();

app.Run();
