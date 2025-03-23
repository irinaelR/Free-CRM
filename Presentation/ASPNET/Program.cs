using Application.Common.Services.DatabaseCleaningManager;
using ASPNET.BackEnd;
using ASPNET.BackEnd.Common.Middlewares;
using ASPNET.FrontEnd;
using Infrastructure.AlertManager;
using Infrastructure.AlertManager.Checking;
using Infrastructure.CsvManager;
using Infrastructure.DatabaseCleaner;

var builder = WebApplication.CreateBuilder(args);

//>>> Create Logs folder for Serilog
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app_data", "logs");
if (!Directory.Exists(logPath))
{
    Directory.CreateDirectory(logPath);
}

builder.Services.AddBackEndServices(builder.Configuration);
builder.Services.AddScoped<IDatabaseCleanerService,DatabaseCleanerService>();
builder.Services.AddScoped<IAlertConfigService,AlertConfigService>();
builder.Services.AddScoped<CsvService>();
builder.Services.AddScoped<CheckBudgetExpenseService>();
builder.Services.AddFrontEndServices();

var app = builder.Build();

app.RegisterBackEndBuilder(app.Environment, app, builder.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseCors();
app.UseMiddleware<GlobalApiExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapFrontEndRoutes();
app.MapBackEndRoutes();

app.Run();
