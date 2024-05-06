internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //var AzureAIAPIAllowSpecificOrigins = "_azureAIAPIAllowSpecificOrigins";

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin", builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            //options.AddPolicy(name: AzureAIAPIAllowSpecificOrigins,
            //                  policy =>
            //                  {
            //                      policy.WithOrigins("http://localhost:4200");
            //                  });
        });

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseCors("AllowAnyOrigin");

        app.Run();

    }
}