using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using YusurIntegration.Data;
using YusurIntegration.Hubs;
using YusurIntegration.Repositories;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;




var builder = WebApplication.CreateBuilder(args);


string logPath;

if(OperatingSystem.IsWindows())
    logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "yusur-webhook-.txt");
else
    logPath = "/var/www/yusurapi/Logs/yusur-webhook-.txt";


//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .WriteTo.File(logPath,
//        rollingInterval: RollingInterval.Day,
//        retainedFileCountLimit: 30,
//        shared: true) // Add this for file sharing
//    .CreateLogger();

//try
//{
//    Log.Logger = new LoggerConfiguration()
//        .MinimumLevel.Debug()
//        .Enrich.FromLogContext()
//        .WriteTo.Console()
//        .WriteTo.File(
//            logPath,
//            rollingInterval: RollingInterval.Day,
//            flushToDiskInterval: TimeSpan.FromSeconds(1)) // <--- ADD THIS LINE
//        .CreateLogger();
//    Log.Information("Logger initialized successfully at: {LogPath}", logPath);




//}
//catch (Exception ex)
//{
//    Console.WriteLine("Failed to initialize logger: " + ex.Message);
//    Console.WriteLine($"Attempted path: {logPath}");
//    throw;
//}




builder.Host.UseSerilog();


builder.Services.Configure<SignatureValidationService>(builder.Configuration.GetSection("YusurKeys"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseFirebird(builder.Configuration.GetConnectionString("Firebirdyusur")));



builder.Services.AddScoped<IApprovedDrugRepository, ApprovedDrugRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IWasfatyDrugRepository, WasfatyDrugRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IPendingMessageRepository, PendingMessageRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();


builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IOrderValidationService, OrderValidationService>();
builder.Services.AddScoped<IApprovedDrugService, ApprovedDrugService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

builder.Services.AddScoped <IDatabaseService, DatabaseService>();
 


builder.Services.AddSingleton<ConnectionManager>();


builder.Services.AddSignalR();

builder.Services.AddHttpClient<IYusurApiClient, YusurApiClient>();

builder.Services.AddScoped<SignatureValidationService>();



builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This stops the 'mirror in a mirror' crash
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWinFormsClients", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*");
    });
});



//builder.Services.AddHealthChecks()
//    .AddCheck("Yusur-Database-Check", () =>
//    {
//        try
//        {
//            using var connection = new FirebirdSql.Data.FirebirdClient.FbConnection(
//                builder.Configuration.GetConnectionString("Firebirdyusur"));
//            connection.Open();
//            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
//        }
//        catch (Exception ex)
//        {
//            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(ex.Message);
//        }
//    });

builder.Services.AddHealthChecks();

//.AddCheck("Yusur-Database-Check", () =>
// {
//     try
//     {
//         using var connection = new FirebirdSql.Data.FirebirdClient.FbConnection(
//             builder.Configuration.GetConnectionString("Firebirdlocal"));
//         connection.Open();
//         return HealthCheckResult.Healthy();
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine(ex.ToString()); // TEMP
//         return HealthCheckResult.Unhealthy(ex.Message);
//     }
// });





var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

//added for linuxFore
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});







// recently added for linuxFore
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//added for linuxFore


app.UseForwardedHeaders();
app.UseCors("AllowWinFormsClients");


//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
    app.MapScalarApiReference();
//}

 

app.MapControllers();

app.MapHealthChecks("/health");



app.MapHub<YusurHub>("/yusurhub");

try
{
    await app.RunAsync();
    Log.Information("API Starting Up in {Environment}", app.Environment.EnvironmentName);
   
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}




// Add services to the container.

//////builder.Services.AddControllers();
//////// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//////builder.Services.AddOpenApi();

//////var app = builder.Build();

//////// Configure the HTTP request pipeline.
//////if (app.Environment.IsDevelopment())
//////{
//////    app.MapOpenApi();
//////}

//////app.UseAuthorization();

//////app.MapControllers();

//////app.Run();
