using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using YusurIntegration.Data;
using YusurIntegration.Hubs;
using YusurIntegration.Repositories;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);



builder.Services.Configure<SignatureValidationService>(builder.Configuration.GetSection("YusurKeys"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseFirebird(builder.Configuration.GetConnectionString("Firebirdyusur")));

//builder.Services.AddDbContext<StockDbContext>(options =>
//    options.UseFirebird(builder.Configuration.GetConnectionString("Firebirdstock")));

//builder.Services.AddDbContext<ProductsDb>(options =>
//    options.UseFirebird(builder.Configuration.GetConnectionString("FirebirdProducts")));

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


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    //var db2 = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    //db2.Database.Migrate();
}


app.UseCors("AllowWinFormsClients");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<YusurHub>("/yusurhub");
app.Run();




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
