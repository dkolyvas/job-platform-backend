using JobPlatform.Data;
using JobPlatform.Repositories;
using JobPlatform.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using JobPlatform.Services;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
object value = builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "Logs/logs.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {SourceContext}" +
            "[{Level}] {Message}{NewLine}{Exception}",
            retainedFileCountLimit: null,
            fileSizeLimitBytes: null
        );
});
builder.Services.AddDbContext<JobplatformContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAppServices, AppServices>();
builder.Services.AddAutoMapper(typeof(MapperConfig));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        byte[] signingKeyBytes = Encoding.UTF8
        .GetBytes(builder.Configuration.GetValue<string>("AppSecurityKey")!);

        x.IncludeErrorDetails = true; ;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            /*ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            SignatureValidator = (token, validator) => { return new JwtSecurityToken(token); }*/
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:5001",
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
