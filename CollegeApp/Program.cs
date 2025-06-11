using CollegeApp.Configurations;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
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
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));
builder.Services.AddDbContext<CollegeDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeAppDBConnection"));
});
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
//builder.Services.AddCors(options => options.AddPolicy("MyTestCORS", policy =>
//{
//    //allow all origins
//    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//    //specific origin
//    policy.WithOrigins("http://localhost:4248").AllowAnyOrigin().AllowAnyMethod();
//}));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

    });
    options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

});
    options.AddPolicy("AllowOnlylocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
    options.AddPolicy("AllowOnlyGoogle", policy =>
    {
        policy.WithOrigins("http://google.com","http://gmail.com","http://drive.google.com").AllowAnyHeader().AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.UseCors("MyTestCORS");

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("api/testingendpoint",
        context => context.Response.WriteAsync("Test Response")).RequireCors("AllowOnlyLocalhost");

    endpoints.MapControllers().RequireCors("AllowAll");

    endpoints.MapGet("api/testingendpoint2", context => context.Response.WriteAsync("Test Response2"));

});

app.MapControllers();

app.UseSwaggerUi(options =>
{
    options.DocumentPath = "openapi/v1.json";
});

app.Run();
