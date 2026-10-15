using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PNMaterialsAPI.ErrorHandling;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IMaterialCodeGenerator, MaterialCodeGenerator>();
    builder.Services.AddScoped<IRequestNumberGenerator, RequestNumberGenerator>();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    var connection = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));

    app.Logger.LogInformation(
        "Сервер запускается. Версия {Version}, окружение {Environment}, база данных {Database} на {DataSource}",
        typeof(Program).Assembly.GetName().Version?.ToString(),
        app.Environment.EnvironmentName,
        connection.InitialCatalog,
        connection.DataSource);

    app.UseSerilogRequestLogging();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseExceptionHandler();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "PN-Materials API"));
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Неожиданная ошибка во время запуска приложения");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;