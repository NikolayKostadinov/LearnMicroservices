namespace ShoppingCart;

using Polly;
using ShoppingCart;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //configure services
        builder.Services.AddControllers();
        builder.Services.Scan(selector =>
            selector
                .FromAssemblyOf<Program>()
                .AddClasses()
                .AsMatchingInterface());

        builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
            .AddTransientHttpErrorPolicy(p =>
                p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));

        var app = builder.Build();

        app.UseHttpsRedirection()
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());
        app.Run();
    }
}
