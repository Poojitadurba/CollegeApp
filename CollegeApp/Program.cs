using CollegeApp.Data;
using CollegeApp.MyLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
 builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

//Log.Logger = new LoggerConfiguration().
//    MinimumLevel.Information().
//    WriteTo.File("Log/log.txt",rollingInterval:RollingInterval.Minute).CreateLogger();

////builder.Host.UseSerilog();
//builder.Logging.AddSerilog(); //use serilog along with built in loggers

builder.Logging.AddLog4Net(); //use log4net with inbuilt loggers

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddControllers().AddNewtonsoftJson(); //HttpPatch
builder.Services.AddControllers(options=>options.ReturnHttpNotAcceptable=false).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters(); //Content negotiation
builder.Services.AddScoped<IMyLogger, LogToFile>();
builder.Services.AddDbContext<CollegeDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeAppDBConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwaggerUi(options =>
{
    options.DocumentPath = "openapi/v1.json";
});

app.Run();
