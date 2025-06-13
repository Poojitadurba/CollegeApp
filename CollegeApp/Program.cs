using CollegeApp.Configurations;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

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
//builder.Services.AddOpenApi();
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


//string LocalAudience = builder.Configuration.GetValue<string>("LocalAudience");
//string LocalIssuer = builder.Configuration.GetValue<string>("LocalIssuer");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecret"))),
        ValidateIssuer = false,
        //ValidIssuer = LocalIssuer,
        ValidateAudience = false,
        //ValidAudience=LocalAudience
    };
});

builder.Services.AddSwaggerGen(options=>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description ="JWT Authorization header using the bearer scheme. Enter Bearer [space] add your token in the text input. Bearer sosoksowkso",
        Name="Authorization",
        In=ParameterLocation.Header,
        Scheme="bearer",
        Type=SecuritySchemeType.Http,
        BearerFormat="JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Id="Bearer",
                    Type=ReferenceType.SecurityScheme
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
        
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseHttpsRedirection();

//app.UseCors("MyTestCORS");

app.UseRouting();

app.UseAuthentication();

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    //options.DocumentPath = "openapi/v1.json";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CollegeApp API V1");
});


app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("api/testingendpoint",
        context => context.Response.WriteAsync("Test Response")).RequireCors("AllowOnlyLocalhost");

    endpoints.MapControllers().RequireCors("AllowAll");

    endpoints.MapGet("api/testingendpoint2", context => context.Response.WriteAsync(builder.Configuration.GetValue<string>("JWTSecret") ));

});

//app.MapControllers();


app.Run();
