using Hangfire;
using HigzTrade.Application;
using HigzTrade.Infrastructure;
using HigzTrade.TradeApi.Constants;
using HigzTrade.TradeApi.Extensions;
using HigzTrade.TradeApi.Helpers;
using HigzTrade.TradeApi.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
//Register & Configure

// บังคับ Environment ตามโหมดการคอมไพล์
#if DEBUG
    builder.Environment.EnvironmentName = "Development";
#elif RELEASE
    builder.Environment.EnvironmentName = "Staging";
#elif PRODUCTION
    builder.Environment.EnvironmentName = "Production";
#endif



builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);// อ่าน config จาก appsettings.json ทั้งหมดอัตโนมัติ
});

// configuration environment 
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);//global configuration 
if (builder.Environment.IsProduction())
{
    // เฉพาะ Development เท่านั้น
    // เฉพาะ Production
    builder.Configuration
        .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true) /*optional: true เป็นข้อมูล sensitive ไม่ต้องมีใน project dev ก็ได้*/
        .AddEnvironmentVariables(); // ดึงจาก server environment variables (นิยมใน production)
                                    // หรือ Azure Key Vault, AWS Secrets Manager ถ้า deploy cloud
}
else // builder.Environment.IsStaging(), builder.Environment.IsDevelopment()
{
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: false,
        reloadOnChange: true)
        .AddUserSecrets<Program>();// เก็บ secret ใน local เช่น Connection String, API Key

    builder.Services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HigzTrade API",
            Version = AppVersionHelpers.GetBuildVersion()
        });
    });
}





/*
 * Add services to the container.
 *  Register Dependency
 */
builder.Services.AddInfrastructure(builder.Configuration); //register repository, unit of work, dbContext
builder.Services.AddApplication(); //register application service



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); 


/*
 Security Configs
 */
builder.Services.AddRequestTimeouts(options => {
    // ตั้งค่าเริ่มต้นให้ทุก Request ต้องจบภายใน 15 วินาที (ป้องกัน Slowloris Attack และป้องกัน thread ไปค้างที่ request บางตัวที่ช้าผิดปกติ )
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(15),
        TimeoutStatusCode = 504
    };

    // สำหรับ process ที่ต้องประมวลผลนานกว่าปกติ
    //options.AddPolicy(RequestTimeoutCustomPolicy.BigDataProcessingPolicy, new RequestTimeoutPolicy
    //{
    //    Timeout = TimeSpan.FromMinutes(2),
    //    TimeoutStatusCode = 504
    //});
});


var app = builder.Build();
// Configure the HTTP request pipeline.  

//ตัว Log Request (Serilog.AspNetCore)
//app.UseSerilogRequestLogging(options => options.ConfigCustomOptions());
app.UseSerilogRequestLogging(options => { 
    options.MessageTemplate = "{Title} at {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms"; 
});

//ตัวจับ Error
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCustomExceptionHandler();



if (! builder.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
}

app.UseHangfireDashboard(); // hangfire dashboard


app.UseHttpsRedirection();// บังคับให้ทุกๆ request ใช้ https อัตโนมัติ
//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// ** RateLimit (to endpoint or page)
//app.MapGet("/api/resource", () => "This endpoint is rate limited")
//   .RequireRateLimiting("fixed"); // Apply specific policy to an endpoint


app.Run();


