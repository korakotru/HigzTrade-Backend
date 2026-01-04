using HigzTrade.Application;
using HigzTrade.Infrastructure;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);// อ่าน config จาก appsettings.json ทั้งหมดอัตโนมัติ
});

/*
 * Add services to the container.
 *  Register Dependency
 */

builder.Services.AddInfrastructure(builder.Configuration); //register repository, unit of work, dbContext
builder.Services.AddApplication(); //register application service

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



/*
 Security Configs
 */
builder.Services.AddRequestTimeouts(options => {
    // ตั้งค่าเริ่มต้นให้ทุก Request ต้องจบภายใน 15 วินาที (ป้องกัน Slowloris Attack และป้องกัน thread ไปค้างที่ request บางตัวที่ช้าผิดปกติ )
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(15)
    };
});

// ** RateLimit (to endpoint or page)
//app.MapGet("/api/resource", () => "This endpoint is rate limited")
//   .RequireRateLimiting("fixed"); // Apply specific policy to an endpoint


var app = builder.Build();


app.UseSerilogRequestLogging(options =>
{
    // Log ทุก HTTP Request
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserId", httpContext.User?.Identity?.Name ?? "Anonymous");
    };
});


// Configure the HTTP request pipeline. 
// configuration environment
if (builder.Environment.IsDevelopment())
{
    // เฉพาะ Development เท่านั้น
    app.UseSwagger();
    app.UseSwaggerUI();
    builder.Configuration
        .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<Program>(); // เก็บ secret ใน local เช่น Connection String, API Key
}
else if (builder.Environment.IsStaging())
{
    // เฉพาะ Staging (pre-production) *ถ้ามี
    builder.Configuration
        .AddJsonFile("appsettings.Staging.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<Program>(); // เก็บ secret ใน local เช่น Connection String, API Key
}
else
{
    // เฉพาะ Production
    builder.Configuration
        .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true) /*optional: true เป็นข้อมูล sensitive ไม่ต้องมีใน project dev ก็ได้*/
        .AddEnvironmentVariables(); // ดึงจาก server environment variables (นิยมใน production)
                                    // หรือ Azure Key Vault, AWS Secrets Manager ถ้า deploy cloud
}

// ส่วนนี้เหมือนกันทุก environment (global configuration)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);





app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();