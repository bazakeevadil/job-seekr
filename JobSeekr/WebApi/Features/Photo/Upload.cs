namespace WebApi.Features.Photo;

public class UploadFhotoEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/upload", async (HttpRequest request) =>
        {
            using (var reader = new StreamReader(request.Body, System.Text.Encoding.UTF8))
            {
                string fileContent = await reader.ReadToEndAsync();

                return "File Was Processed Sucessfully!";
            }
        })
        .Accepts<IFormFile>("text/plain");
    }
}

public static class Upload
{

}
