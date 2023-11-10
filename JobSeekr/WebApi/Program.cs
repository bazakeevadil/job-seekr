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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapCarter();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();
