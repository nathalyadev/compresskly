using CompressKly.Api.Extensions;
using CompressKly.Api.Middlewares;
using CompressKly.Infrastructure;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCompressKlyControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCompressKlyOpenApi();

var maxFileSizeMb = builder.Configuration.GetValue("Upload:MaxFileSizeMb", 500);
var maxFileSizeBytes = maxFileSizeMb * 1024L * 1024L;

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxFileSizeBytes;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxFileSizeBytes;
});

builder.Services.AddCompressKlyServices(builder.Configuration);
builder.Services.AddTransient<GlobalExceptionMiddleware>();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Front", policy =>
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseCompressKlyOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("Front");
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program;
