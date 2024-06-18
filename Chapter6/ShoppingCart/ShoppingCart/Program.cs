using Polly;
using ShoppingCart.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
    .AddTransientHttpErrorPolicy(p => p
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));


builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(ctx => ctx.Where(c => c.Name.EndsWith("Store")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    .AddClasses(ctx => ctx.Where(c => c.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    .AddClasses(ctx => ctx.Where(c => c.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
