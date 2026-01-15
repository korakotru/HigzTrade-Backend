using Hangfire;
using HigzTrade.Application;
using HigzTrade.Infrastructure;
using HigzTrade.TradeApi.Extensions;
using HigzTrade.TradeApi.Helpers;
using HigzTrade.TradeApi.Middlewares;
using Mapster;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;


var builder = WebApplication.CreateBuilder(args);
//register services/configurations เข้าไปใน DI container (Dependency Injection)

// บังคับ Environment ตามโหมดการคอมไพล์
#if DEBUG
builder.Environment.EnvironmentName = "Development";
#elif RELEASE
    builder.Environment.EnvironmentName = "Staging";
#elif PRODUCTION
    builder.Environment.EnvironmentName = "Production";
#endif

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClientPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:5173") // Vite Port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);// อ่าน config จาก appsettings.json ทั้งหมดอัตโนมัติ
});

// configuration environment 
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);//global configuration 
if (builder.Environment.IsProduction())
{
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

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HigzTrade API",
            Version = AppVersionHelpers.GetBuildVersion()
        });
        c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    });
}



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // บังคับให้ RequestPath เป็น ตัวพิมพ์เล็กทั้งหมด ป้องกัน server บางตัวเป็น case-sensitive
    options.LowercaseQueryStrings = false;// ไม่แปลง queryString เป็น lowercase
});


/*
 * Add services to the container.
 *  Register Dependency
 */
builder.Services.AddInfrastructure(builder.Configuration); //register repository, unit of work, dbContext
builder.Services.AddApplicationService(); //register application service
builder.Services.AddMapster(); // Auto mapper


/*
 Security Configs
 */
builder.Services.AddCustomRequestTimeouts();
builder.Services.AddCustomRateLimiter();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiSuccessResponseFilter>(); // Customize ApiResponse Format on Success 2xx (no need to wrap response in api controller) 
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // แปลง response เป็น camelCase อัตโนมัติ
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // แปลง enum เป็น string แทนตัวเลข อัตโนมัติ
});

/**********************************************************************************/
var app = builder.Build();
// Configure pipeline. (by sequence)

app.UseCors("WebClientPolicy"); // ต้องวางก่อน UseAuthorization

app.UseRateLimiter(); // Security policy first


//ตัว Log Request (Serilog.AspNetCore)
//app.UseSerilogRequestLogging(options => options.ConfigCustomOptions());
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{Title} at {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
    {
        // 1. กรอง: ถ้าเป็น GET ไม่ต้องอ่าน Body ให้เสียแรงครับ
        if (httpContext.Request.Method != "POST" && httpContext.Request.Method != "PUT")
            return;

        // 2. กรอง: ถ้าเป็น Path ของการ Upload ไฟล์ ก็ข้ามไปเลยครับ
        if (httpContext.Request.Path.StartsWithSegments("/api/Upload"))
            return;


        httpContext.Request.EnableBuffering();

        // 2. ใช้ PipeReader (ถ้าใช้ .NET Core 3.0+) จะเร็วกว่า StreamReader 
        // แต่ถ้าเอาเขียนง่าย StreamReader แบบเดิมก็พอไหว 
        //if (httpContext.Request.ContentLength < 51200)
        //{
        //    using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
        //    var body = await reader.ReadToEndAsync();
        //    diagnosticContext.Set("RequestBody", body);
        //    httpContext.Request.Body.Position = 0;
        //}
    };
});

//ตัวจับ Error
app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseCustomExceptionHandler();


if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
}

app.UseHangfireDashboard(); // hangfire dashboard


app.UseHttpsRedirection();// บังคับให้ทุกๆ request ใช้ https อัตโนมัติ
//app.UseAuthentication();
//app.UseAuthorization();
app.MapControllers();


/**********************************************************************************/
app.Run();


