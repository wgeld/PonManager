using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Server.Data.Services;
using TestClientServer.Shared.Models.DBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WcfMgmtTestContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WcfMgmtTestContext")));

builder.Services.AddScoped<IAvailableSignalPorts2Service, AvailableSignalPorts2Service>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IUtilityService, UtilityService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
