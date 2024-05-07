internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var AzureAIAPIAllowSpecificOrigins = "_azureAIAPIAllowSpecificOrigins";

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AzureAIAPIAllowSpecificOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("https://budget-bites.azurewebsites.net/");
                              });
        });

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseCors(AzureAIAPIAllowSpecificOrigins);

        app.Run();

    }
}